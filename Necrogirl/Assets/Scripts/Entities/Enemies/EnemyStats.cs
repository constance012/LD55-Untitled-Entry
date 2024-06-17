using UnityEngine;

public abstract class EnemyStats : EntityStats
{
	[Header("Attacking Settings"), Space]
	public float attackRadius;
	[SerializeField] protected LayerMask hitLayer;

	[Header("Health Bar"), Space]
	[SerializeField] protected WorldHealthBar healthBar;
	
	[Header("Animator"), Space]
	[SerializeField] protected Animator animator;


	// Protected fields.
	protected Collider2D[] _hitObjects = new Collider2D[2];
	protected ContactFilter2D _contactFilter;

	private void Awake()
	{
		_mat = this.GetComponentInChildren<SpriteRenderer>("Graphic").material;
	}

	protected override void Start()
	{
		base.Start();

		_contactFilter.layerMask = hitLayer;
		_contactFilter.useLayerMask = true;

		healthBar.SetMaxHealth(stats.GetDynamicStat(Stat.MaxHealth));
		healthBar.name = $"{gameObject.name} Health Bar";
	}

	private void Update()
    {
        TryAttack();
    }

    protected override void TryAttack()
    {
        _attackInterval -= Time.deltaTime;

        if (_attackInterval <= 0f && !PlayerStats.IsDeath)
        {
			if (_attackCoroutine != null)
				StopCoroutine(_attackCoroutine);

            _attackCoroutine = StartCoroutine(DoAttack());
        }
    }

    public override void TakeDamage(float amount, bool weakpointHit, Vector3 attackerPos = default, float knockBackStrength = 0)
	{
		base.TakeDamage(amount, weakpointHit, attackerPos, knockBackStrength);

		healthBar.SetCurrentHealth(_currentHealth);
	}

    public override void Heal(float amount)
    {
        base.Heal(amount);

		healthBar.SetCurrentHealth(_currentHealth);
    }

    public override void Die()
	{
		Destroy(healthBar.gameObject);

		base.Die();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}
}
