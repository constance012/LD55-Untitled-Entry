using UnityEngine;

public class MeleeEnemyAI : EntityAI
{
	[Header("Wandering Settings"), Space]
	[SerializeField] private StatsUpgrade wanderSpeedUpgrade;
	[SerializeField, Tooltip("The amount of time before picking a new wandering destination")]
	private Vector2 wanderDelayRange;

	// Protected fields.
	protected EnemyStats _enemyStats;

	// Private fields.
	private EnemySpawner _spawnPoint;
	private Vector2 _wanderDestination;
	private float _wanderDelay = 0f;

	protected override void Start()
	{
		base.Start();
		_enemyStats = heart as EnemyStats;
	}

    protected override void FixedUpdate()
    {
		base.FixedUpdate();

		if (LocateTarget() && !PlayerStats.IsDeath)
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

	public override void TryAlertTarget(float distanceToTarget, bool forced = false)
	{
		if (distanceToTarget <= aggroRange)
		{
			_spotTimer -= Time.deltaTime;
			if ((_spotTimer <= 0f && !_abandonedTarget) || forced)
			{
				if (forced)
					_abandonedTarget = false;
				
				_spottedTarget = true;
				_spotTimer = 0f;

				_nearbyEntities.Add(rb2D);
			}
		}
		else
			_spotTimer = spotTimer;
	}

	protected override void TryAbandonTarget(float distanceToTarget)
	{
		bool targetTooFarAway = distanceToTarget > _spawnPoint.range + _enemyStats.attackRadius * 2;

		if (distanceToTarget > _enemyStats.attackRadius || targetTooFarAway)
		{
			_abandonTimer -= Time.deltaTime;
			if (_abandonTimer <= 0f && target != null)
			{
				StopFollowingPath();
				
				_nearbyEntities.Remove(rb2D);
				_inAggroTargets.Clear();
				_abandonedTarget = true;
				_abandonTimer = ScaledAbandonTimer;
			}
		}
		else
			_abandonTimer = ScaledAbandonTimer;
	}

	private void WanderAround()
	{
		if (_wanderDelay <= 0f)
		{
			_wanderDestination = _spawnPoint.position + Random.insideUnitCircle * _spawnPoint.range;
			_wanderDelay = Random.Range(wanderDelayRange.x, wanderDelayRange.y);
			_finishedFollowingPath = false;
		}
		else
		{
			if (!_finishedFollowingPath)
			{
				RequestNewPath(_wanderDestination);
			}
			else
			{
				_wanderDelay -= Time.deltaTime;
				_abandonedTarget = false;
			}
		}
	}
}