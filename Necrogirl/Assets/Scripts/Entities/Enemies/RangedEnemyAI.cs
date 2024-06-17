using UnityEngine;

public class RangedEnemyAI : MeleeEnemyAI
{
	[Header("Keep Distance Settings"), Space]
	[SerializeField] private float retreatMinDistance;

	protected override void ProcessTarget()
	{
		float distance = Vector3.Distance(target.position, transform.position);
		_forcedStopMoving = distance <= _enemyStats.attackRadius;

		if (distance <= retreatMinDistance)
		{
			Vector2 direction = (transform.position - target.position).normalized;
			Vector2 velocity = CalculateVelocity(direction);
			
			CheckFlip();

			rb2D.velocity = velocity;
		}
		else if (distance >= _enemyStats.attackRadius)
		{
			_forcedStopMoving = false;
			RequestNewPath(target.position);
		}
	}

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, retreatMinDistance);
    }
}