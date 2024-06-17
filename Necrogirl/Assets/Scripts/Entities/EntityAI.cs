using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityAI : Seeker
{
	[Header("References"), Space]
	[SerializeField] protected Rigidbody2D rb2D;
	[SerializeField] protected Animator animator;
	[SerializeField] protected EntityStats heart;
	[SerializeField] protected Stats stats;

	[Header("Mobility Settings"), Space]
	[SerializeField] private float repelRange;
	[SerializeField] private float repelAmplitude;

	[Header("Spotting Settings"), Space]
	[SerializeField] protected float aggroRange;
	[SerializeField] protected float spotTimer;
	[SerializeField] protected float standingStillTimeout;
	[SerializeField] protected LayerMask spotLayer;

	[Header("Abandon Settings"), Space]
	[SerializeField] protected float abandonTimer;
	[SerializeField, Range(0f, 1f)] protected float decrementPerTarget;

	// Properties.
	protected float ScaledAbandonTimer => abandonTimer * Mathf.Max(.2f, 1f - decrementPerTarget * _inAggroTargets.Count);

	// Protected fields.
	protected static HashSet<Rigidbody2D> _nearbyEntities;
	protected readonly Collider2D[] _hitTargets = new Collider2D[5];
	protected readonly List<EntityStats> _inAggroTargets = new List<EntityStats>();
	protected ContactFilter2D _contactFilter;
	protected Vector2 _targetPreviousPos;
	protected bool _facingRight = true;
	protected bool _forcedStopMoving;
	protected bool _spottedTarget;
	protected float _spotTimer;
	protected float _standingStillTimeout;
	protected float _abandonTimer;
	protected bool _abandonedTarget;

	protected virtual void Start()
	{
		_nearbyEntities ??= new HashSet<Rigidbody2D>();

		_targetPreviousPos = transform.position;

		_contactFilter.layerMask = spotLayer;
		_contactFilter.useLayerMask = true;

		_spotTimer = spotTimer;
		_standingStillTimeout = standingStillTimeout;
		_abandonTimer = abandonTimer;
	}

	private void OnDestroy()
	{
		_nearbyEntities.Remove(rb2D);
	}

	protected virtual void FixedUpdate()
	{
		animator.SetFloat("Speed", rb2D.velocity.sqrMagnitude);

		if (animator.GetFloat("Speed") < .04f)
			_standingStillTimeout -= Time.deltaTime;
		else
			_standingStillTimeout = standingStillTimeout;
	}

	protected void RequestNewPath(Vector3 toPos)
	{
		// Request a path if the target has moved a certain distance fron the last position.
		if (Vector3.Distance(toPos, _targetPreviousPos) >= maxMovementDelta || _standingStillTimeout <= 0f)
		{
			PathRequester.Request(new PathRequestData(transform.position, toPos, this.gameObject, OnPathFound));
			_targetPreviousPos = toPos;
			_standingStillTimeout = standingStillTimeout;
		}
	}
	
	protected bool LocateTarget()
    {
		float distanceToTarget = target != null ? Vector2.Distance(transform.position, target.position) :
												Vector2.Distance(transform.position, PlayerMovement.Position);

		if (!_spottedTarget)
			TryAlertTarget(distanceToTarget);
		else
			TryAbandonTarget(distanceToTarget);
		
		return _spottedTarget;
	}

	protected void ChaseTarget()
	{	
		if (TrySelectTarget() && target != null)
			ProcessTarget();
		else
		{
			if (_inAggroTargets.Count > 0)
				_inAggroTargets.Clear();
			
			_forcedStopMoving = false;
			_spottedTarget = false;
			_spotTimer = spotTimer;
		}
	}

	protected virtual void ProcessTarget()
	{
		RequestNewPath(target.position);
	}

	protected bool TrySelectTarget()
	{
		int hitColliders = Physics2D.OverlapCircle(transform.position, aggroRange, _contactFilter, _hitTargets);

		if (hitColliders > 0)
		{
			_inAggroTargets.Clear();
			for (int i = 0; i < hitColliders; i++)
			{
				EntityStats entity = _hitTargets[i].GetComponentInParent<EntityStats>();

				if (entity != null && entity.transform != target)
					_inAggroTargets.Add(entity);
			}
			
			// Sort by priority if the list is not empty.
			if (_inAggroTargets.Count > 0)
			{
				// Locate new target only if the current one is dead or abandoned.
				if (_abandonedTarget || target == null)
				{
					_inAggroTargets.Sort();

					// Only assign new target if it's different from the previous one.
					if (target != _inAggroTargets[0].transform)
					{
						target = _inAggroTargets[0].transform;
						_abandonedTarget = false;
					}
				}	
			}

			return !_abandonedTarget;
		} 

		return false;
	}
	
	protected abstract void TryAbandonTarget(float distanceToTarget);
	public abstract void TryAlertTarget(float distanceToTarget, bool forced = false);

	protected override IEnumerator ExecuteFoundPath(int previousIndex = -1)
	{
		if (_path.Length == 0)
			yield break;

		Vector2 currentWaypoint = previousIndex == -1 ? _path[0] : _path[previousIndex];

		//Debug.Log($"{gameObject.name} following path...");

		while (!_forcedStopMoving && _path.Length > 0)
		{
			float distanceToCurrent = Vector2.Distance(rb2D.position, currentWaypoint);

			if (distanceToCurrent <= .15f)
			{
				_waypointIndex++;

				// If there's no more waypoints to move, then simply returns out of the coroutine.
				if (_waypointIndex >= _path.Length)
				{
					StopFollowingPath();
					yield break;
				}

				currentWaypoint = _path[_waypointIndex];
			}

			Vector2 direction = (currentWaypoint - rb2D.position).normalized;
			Vector2 velocity = CalculateVelocity(direction);

			CheckFlip();

			rb2D.velocity = velocity;

			yield return new WaitForFixedUpdate();
		}

		StopFollowingPath();
	}

	protected void StopFollowingPath()
	{
		Debug.Log("Finished following path.");

		_finishedFollowingPath = true;
		_waypointIndex = 0;
		_path = new Vector3[0];
		rb2D.velocity = Vector2.zero;
	}

	protected void CheckFlip()
	{
		float sign = target != null ? Mathf.Sign(target.position.x - rb2D.position.x) : Vector2.Dot(rb2D.velocity.normalized, Vector2.right);
		bool mustFlip = (_facingRight && sign < 0f) || (!_facingRight && sign > 0f);

		if (mustFlip)
		{
			transform.Rotate(Vector3.up * 180f);
			_facingRight = !_facingRight;
		}
	}

	/// <summary>
	/// Calculate the final velocity after applying repel force against other entities nearby.
	/// </summary>
	/// <param name="direction"></param>
	/// <param name="externalMultiplier"></param>
	/// <returns></returns>
	protected Vector2 CalculateVelocity(Vector2 direction)
	{
		// Enemies will try to avoid each other.
		Vector2 repelForce = Vector2.zero;

		foreach (Rigidbody2D entity in _nearbyEntities)
		{
			if (entity == rb2D || entity.velocity.sqrMagnitude < 1f)
				continue;

			if (Vector2.Distance(entity.position, rb2D.position) <= repelRange)
			{
				Vector2 repelDirection = (rb2D.position - entity.position).normalized;
				repelForce += repelDirection;
			}
		}

		Vector2 velocity = stats.GetDynamicStat(Stat.MoveSpeed) * direction;
		velocity += repelForce.normalized * repelAmplitude;

		return velocity;
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, repelRange);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, aggroRange);
	}
}