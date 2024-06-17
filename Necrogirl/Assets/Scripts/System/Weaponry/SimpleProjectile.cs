using UnityEngine;

public sealed class SimpleProjectile : ProjectileBase
{
	private void OnCollisionEnter2D(Collision2D other)
	{
		flySpeed = 0f;

		ProcessCollision(other);

		Destroy(gameObject);
	}

	public override void ProcessCollision(Collision2D other)
	{
		EntityStats target = other.collider.GetComponentInParent<EntityStats>();

		if (target != null && _wearer != null)
		{
			target.TakeDamage(_wearerStats.GetDynamicStat(Stat.Damage), false, _wearer.transform.position, _wearerStats.GetStaticStat(Stat.KnockBackStrength));

			float lifeStealRatio = _wearerStats.GetStaticStat(Stat.LifeStealRatio);
			if (lifeStealRatio != -1f)
				_wearer.Heal(Mathf.Ceil(_wearerStats.GetDynamicStat(Stat.Damage) * lifeStealRatio));
		}
	}
}
