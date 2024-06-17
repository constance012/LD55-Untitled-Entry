using System.Collections;
using UnityEngine;

public class RangedEnemy : EnemyStats
{
	[Header("Projectile Prefab"), Space]
	[SerializeField] private GameObject projectilePrefab;

	// Private fields.
	private MeleeEnemyAI _enemyBrain;

	protected override void Start()
	{
		base.Start();
		_enemyBrain = brain as MeleeEnemyAI;
	}

	protected override IEnumerator DoAttack()
	{
		Transform currentTarget = _enemyBrain.target;

		if (currentTarget != null && Vector3.Distance(currentTarget.position, transform.position) <= attackRadius)
		{
			rb2D.velocity = Vector2.zero;

			brain.enabled = false;

			if (rpgClass == EntityRPGClass.Magic)
				animator.Play("Use Spell");

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