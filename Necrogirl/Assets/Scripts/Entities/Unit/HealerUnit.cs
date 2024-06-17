using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerUnit : UnitStats
{
	// Private fields.
	private static HashSet<HealerUnit> _onFieldHealers;
	private float _baseHealEfficiency = 1f;

    protected override void Start()
    {
        base.Start();

		_attackInterval = BaseAttackInterval;

		_hitObjects = new Collider2D[7];
		_onFieldHealers ??= new HashSet<HealerUnit>();
		_onFieldHealers.Add(this);
    }

	private void OnDestroy()
	{
		_onFieldHealers.Remove(this);
	}

    protected override IEnumerator DoAttack()
    {
		int hitColliders = Physics2D.OverlapCircle(transform.position, attackRadius, _contactFilter, _hitObjects);

		if (hitColliders > 0)
		{
			rb2D.velocity = Vector2.zero;

			brain.enabled = false;

			// Calculate the healing efficiency of all the on field healers.
			float efficiency;
			if (_onFieldHealers.Count < 4)
				efficiency = _baseHealEfficiency - (_onFieldHealers.Count - 1) * stats.GetStaticStat(Stat.HealEfficiencyLossRatio);
			else
				efficiency = _baseHealEfficiency / _onFieldHealers.Count;

			for (int i = 0; i < hitColliders; i++)
			{
				EntityStats entity = _hitObjects[i].GetComponentInParent<EntityStats>();

				if (entity != null)
					entity.Heal(Mathf.Ceil(stats.GetDynamicStat(Stat.Damage) * efficiency));
			}

			_attackInterval = BaseAttackInterval;

			yield return new WaitForSeconds(.2f);

			brain.enabled = true;
		}
    }
}