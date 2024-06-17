using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyStats
{
	protected override IEnumerator DoAttack()
	{
		int hitColliders = Physics2D.OverlapBox(transform.position, Vector2.one * attackRadius, 0f, _contactFilter, _hitObjects);

		if (hitColliders > 0)
		{
			rb2D.velocity = Vector2.zero;

			brain.enabled = false;
			animator.Play("Slash");

			for (int i = 0; i < hitColliders; i++)
			{
				EntityStats entity = _hitObjects[i].GetComponentInParent<EntityStats>();

				if (entity != null)
					entity.TakeDamage(stats.GetDynamicStat(Stat.Damage), false, transform.position, stats.GetStaticStat(Stat.KnockBackStrength));

				float lifeStealRatio = stats.GetStaticStat(Stat.LifeStealRatio);
				if (lifeStealRatio != -1f)
					this.Heal(Mathf.Ceil(stats.GetDynamicStat(Stat.Damage) * lifeStealRatio));
			}

			_attackInterval = BaseAttackInterval;

			yield return new WaitForSeconds(.2f);

			brain.enabled = true;
		}
	}
}