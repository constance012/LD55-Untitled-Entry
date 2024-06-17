using UnityEngine;

public abstract class UnitAI : EntityAI
{
    [Header("Force Follow Distance"), Space]
	[SerializeField] private float forceFollowPlayerLimit;

	// Protected fields.
	protected UnitStats _unitStats;
	protected bool _playerTooFarAway;

	protected override void Start()
	{
		base.Start();
		_nearbyEntities.Add(this.rb2D);
		_unitStats = heart as UnitStats;

		animator.Play("Unit Summoned");
	}

	protected override void FixedUpdate()
    {
        if (PlayerStats.IsDeath)
		 	return;

        base.FixedUpdate();

		// Force the unit to follow the player if she goes too far than a certain distance, or when there's no target in sight.
		_playerTooFarAway = Vector3.Distance(transform.position, PlayerMovement.Position) >= forceFollowPlayerLimit;

		if (LocateTarget())
			ChaseTarget();
		else
			RequestNewPath(PlayerMovement.Position);
    }

	public override void TryAlertTarget(float distanceToTarget, bool forced = false)
	{
		if (distanceToTarget <= aggroRange && !_playerTooFarAway)
		{
			_spotTimer -= Time.deltaTime;
			if ((_spotTimer <= 0f && !_abandonedTarget) || forced)
			{
				if (forced)
					_abandonedTarget = false;
				
				_spottedTarget = true;
				_spotTimer = 0f;
			}
		}
		else
			_spotTimer = spotTimer;
	}

	protected override void TryAbandonTarget(float distanceToTarget)
	{
		// If the enemy is dead, then follows the player.
		if (target == null || _playerTooFarAway)
		{
			if (_playerTooFarAway)
			{
				_spottedTarget = false;
				_spotTimer = spotTimer;
				target = null;
			}

			StopFollowingPath();

			_forcedStopMoving = false;
			_abandonTimer = ScaledAbandonTimer;
		}
		else if (distanceToTarget > _unitStats.attackRadius)
		{
			_abandonTimer -= Time.deltaTime;
			if (_abandonTimer <= 0f)
			{
				StopFollowingPath();
				_abandonedTarget = true;
				_abandonTimer = ScaledAbandonTimer;
			}
		}
		else
			_abandonTimer = ScaledAbandonTimer;
	}

	protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, forceFollowPlayerLimit);
    }
}