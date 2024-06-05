using UnityEngine;

public class MeleeEnemyAI : EntityAI
{
	[Header("Wandering Settings"), Space]
	[SerializeField, Range(.2f, .8f)] private float wanderSpeedMultiplier;
	[SerializeField, Tooltip("The amount of time before picking a new wandering destination")]
	private Vector2 wanderDelayRange;

	// Private fields.
	private bool _spottedPlayer;
	private float _spotTimer;
	private EnemySpawner _spawnPoint;
	private Vector2 _wanderDestination;
	private float _wanderDelay = 0f;

	protected override void Start()
	{
		base.Start();
		_spotTimer = spotTimer;
	}

    protected override void FixedUpdate()
    {
		if (PlayerStats.IsDeath)
			return;

		base.FixedUpdate();

		if (SpotTarget())
			ChaseTarget();
		else
			WanderAround();
    }

	public void SetSpawnArea(EnemySpawner spawnPoint)
	{
		_spawnPoint = spawnPoint;
		_wanderDestination = rb2D.position;
		_wanderDelay = Random.Range(wanderDelayRange.x, wanderDelayRange.y);
	}

	public void Alert()
	{
		_spottedPlayer = true;
		_spotTimer = 0f;

		_nearbyEntities.Add(rb2D);
	}

	protected virtual void ChaseTarget()
	{	
		if (TrySelectTarget() && target != null)
			RequestNewPath(target.position);
	}

    protected override bool TrySelectTarget()
    {
		int hitColliders = Physics2D.OverlapCircle(transform.position, aggroRange, _contactFilter, _hitTargets);

		if (hitColliders > 0)
		{
			_inAggroTargets.Clear();
			for (int i = 0; i < hitColliders; i++)
			{
				EntityStats entity = _hitTargets[i].GetComponentInParent<EntityStats>();

				if (entity != null)
					_inAggroTargets.Add(entity);
			}
			
			// Sort by priority if the list is not empty.
			if (_inAggroTargets.Count > 0)
			{
				// Only assign new target if it's different from the previous one or the previous is null.
				if (target == null || target != _inAggroTargets[0].transform)
				{
					_inAggroTargets.Sort();
					target = _inAggroTargets[0].transform;
					Debug.Log(target.name);
				}
			}

			return _inAggroTargets.Count > 0;
		} 

		return false;
    }

	private void WanderAround()
	{
		if (_wanderDelay <= 0f && rb2D.velocity == Vector2.zero)
		{
			_wanderDestination = _spawnPoint.position + Random.insideUnitCircle * _spawnPoint.range;
			_wanderDelay = Random.Range(wanderDelayRange.x, wanderDelayRange.y);
		}
		else
		{
			Vector2 direction = _wanderDestination - rb2D.position;
			if (direction.sqrMagnitude > .2f * .2f)
			{
				Vector2 velocity = CalculateVelocity(direction.normalized, wanderSpeedMultiplier);

				CheckFlip();
				rb2D.velocity = velocity;
			}
			else
			{
				rb2D.velocity = Vector2.zero;
				_wanderDelay -= Time.deltaTime;
			}
		}
	}

    private bool SpotTarget()
    {
        if (!_spottedPlayer)
		{
			float distanceToPlayer = Vector2.Distance(transform.position, PlayerMovement.Position);
			
			if (distanceToPlayer <= aggroRange)
			{
				_spotTimer -= Time.deltaTime;

				if (_spotTimer <= 0)
					Alert();
			}
			else
				_spotTimer = spotTimer;
		}
		
		return _spottedPlayer;
	}
}
