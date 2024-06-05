using System.Collections;
using UnityEngine;

public class RangedUnit : UnitStats
{
	[Header("Projectile Prefab"), Space]
	[SerializeField] private GameObject projectilePrefab;

	// Protected fields.
	protected UnitAI _unitBrain;

    protected override void Start()
    {
        base.Start();
		_unitBrain = brain as UnitAI;
    }

    protected override IEnumerator DoAttack()
    {
		Transform currentTarget = _unitBrain.target;

		if (currentTarget != null && Vector3.Distance(currentTarget.position, transform.position) <= _rangedAttackRadius && !_unitBrain.TargetIsPlayer)
		{
			rb2D.velocity = Vector2.zero;

			brain.enabled = false;

			Vector2 direction = (currentTarget.position - transform.position).normalized;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

			GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
			
			projectile.name = projectilePrefab.name;
			projectile.transform.eulerAngles = Vector3.forward * angle;
			projectile.GetComponent<SimpleProjectile>().Initialize(this, this.stats, currentTarget);

			_attackInterval = BaseAttackInterval;

			yield return new WaitForSeconds(.2f);

			brain.enabled = true;
		}
    }
}