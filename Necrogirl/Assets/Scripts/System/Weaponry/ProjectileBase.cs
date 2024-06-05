using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
	[Header("References")]
	[Space]
	[SerializeField] protected Rigidbody2D rb2D;
	[SerializeField, Tooltip("The target to track if this projectile is homing.")]
	protected Transform targetToTrack;

	[Header("General Properties")]
	[Space]
	[SerializeField] protected float maxLifeTime;
	[SerializeField] protected bool isHoming;
	
	[Header("Movement Properties")]
	[Space]
	[SerializeField] protected float flySpeed;
	[SerializeField, Tooltip("How sharp does the projectile turn to reach its target? Measures in deg/s.")]
	protected float trackingRigidity;

	// Protected fields.
	protected float _aliveTime;
	protected EntityStats _wearer;
	protected Stats _wearerStats;

	protected virtual void Update()
	{
		_aliveTime += Time.deltaTime;

		if (_aliveTime >= maxLifeTime)
		{
			Destroy(gameObject);
		}
	}

	protected virtual void FixedUpdate()
	{
		if (PlayerStats.IsDeath)
		{
			targetToTrack = null;
		 	return;
		}

		TravelForwards();

		TrackingTarget();
	}

	public void SetTarget(Transform target)
	{
		this.targetToTrack = target;
		this.isHoming = this.targetToTrack != null;
	}
	
	public virtual void Initialize(EntityStats wearer, Stats wearerStats, Transform trackTarget)
	{
		_wearer = wearer;
		_wearerStats = wearerStats;
		
		targetToTrack = trackTarget;

		this.flySpeed = _wearerStats.GetStaticStat(Stat.ProjectileSpeed);
		this.trackingRigidity = _wearerStats.GetStaticStat(Stat.ProjectileTrackingRigidity);

		this.maxLifeTime = _wearerStats.GetStaticStat(Stat.ProjectileLifeTime);
	}

	/// <summary>
	/// Determines what happens if this projectile collides with other objects.
	/// </summary>
	/// <param name="other"></param>
	public abstract void ProcessCollision(Collision2D other);

	protected void TravelForwards() => rb2D.velocity = transform.right * flySpeed;

	protected virtual void TrackingTarget()
	{
		if (isHoming && targetToTrack != null)
		{
			Vector3 trackDirection = targetToTrack.position - transform.position;
			float angle = Mathf.Atan2(trackDirection.y, trackDirection.x) * Mathf.Rad2Deg;

			rb2D.rotation = Mathf.MoveTowardsAngle(rb2D.rotation, angle, trackingRigidity * Time.deltaTime);
		}
	}
}
