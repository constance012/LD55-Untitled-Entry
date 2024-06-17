using System.Collections;
using UnityEngine;

public class MeleeUnit : UnitStats
{
	[Header("Animator"), Space]
	[SerializeField] private Animator animator;

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
				EnemyStats enemy = _hitObjects[i].GetComponentInParent<EnemyStats>();

				if (enemy != null)
					enemy.TakeDamage(stats.GetDynamicStat(Stat.Damage), false, transform.position, stats.GetStaticStat(Stat.KnockBackStrength));
			}

			_attackInterval = BaseAttackInterval;

			yield return new WaitForSeconds(.2f);

			brain.enabled = true;
		}
    }
}