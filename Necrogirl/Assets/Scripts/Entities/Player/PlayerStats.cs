using System.Collections;
using UnityEngine;

public class PlayerStats : EntityStats
{
	[Header("Player Stats"), Space]
	[SerializeField, Min(0f)] private float invincibilityTime;

	[Header("Projectile Prefab"), Space]
	[SerializeField] private GameObject projectilePrefab;
	
	[Header("Tooltip Trigger"), Space]
	[SerializeField] private TooltipTrigger trigger;

	public static bool IsDeath { get; set; }
	public float CurrentMana => _currentMana;
	public float MaxMana => stats.GetDynamicStat(Stat.MaxMana);
	public bool IsAlive => _currentHealth > 0f;

	// Private fields
	private float _invincibilityTime;
	private float _currentMana;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ResetStatic()
	{
		IsDeath = false;
	}

	private void Awake()
	{
		_mat = this.GetComponentInChildren<SpriteRenderer>("Graphic").material;
	}

	protected override void Start()
	{
		base.Start();
		_currentMana = stats.GetDynamicStat(Stat.MaxMana);
		
		trigger.content = stats.ToString();
		GameManager.Instance.UpdateHealthBar(stats.GetDynamicStat(Stat.MaxHealth), true);
		SummonManager.Instance.UpdateManaBar(stats.GetDynamicStat(Stat.MaxMana), true);

		_invincibilityTime = invincibilityTime;
		IsDeath = false;
	}

	private void Update()
	{
		if (GameManager.Instance.GameFinished)
			return;

		if (_invincibilityTime > 0f)
			_invincibilityTime -= Time.deltaTime;

		TryAttack();
	}

	protected override void TryAttack()
	{
		_attackInterval -= Time.deltaTime;

		if (_attackInterval <= 0f)
		{
			if (_attackCoroutine != null)
				StopCoroutine(_attackCoroutine);

            _attackCoroutine = StartCoroutine(DoAttack());
		}
	}

    protected override IEnumerator DoAttack()
    {
        if (LegacyInputManager.Instance.GetKeyDown(KeybindingActions.PrimaryAttack))
		{
			rb2D.velocity = Vector2.zero;
			
			brain.enabled = false;

			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 direction = (mousePos - PlayerMovement.Position).normalized;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

			GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
			projectile.name = projectilePrefab.name;
			projectile.transform.eulerAngles = Vector3.forward * angle;
			projectile.GetComponent<SimpleProjectile>().Initialize(this, this.stats, null);

			_attackInterval = BaseAttackInterval;

			yield return new WaitForSeconds(.2f);

			brain.enabled = true;
		}
    }

    public override void TakeDamage(float amount, bool weakpointHit, Vector3 attackerPos = default, float knockBackStrength = 0f)
	{
		if (IsAlive && _invincibilityTime <= 0f)
		{
			base.TakeDamage(amount, weakpointHit, attackerPos, knockBackStrength);

			CameraShaker.Instance.ShakeCamera(2.5f, .1f);
			GameManager.Instance.UpdateHealthBar(_currentHealth);

			_invincibilityTime = invincibilityTime;
		}
	}

	public override void Heal(float amount)
	{
		if (IsAlive)
		{
			base.Heal(amount);

			GameManager.Instance.UpdateHealthBar(_currentHealth);
		}
	}

	public void ConsumeMana(float manaCost)
	{
		if (IsAlive)
		{
			_currentMana = Mathf.Max(_currentMana - manaCost, 0f);

			DamageText.Generate(dmgTextPrefab, dmgTextLoc.position, DamageText.ManaColor, DamageTextStyle.Normal, $"-{manaCost}");
			SummonManager.Instance.UpdateManaBar(_currentMana);
		}
	}

	public void RecoverMana(float amount)
	{
		if (IsAlive)
		{
			_currentMana = Mathf.Min(_currentMana + amount, stats.GetDynamicStat(Stat.MaxMana));

			DamageText.Generate(dmgTextPrefab, dmgTextLoc.position, DamageText.ManaColor, DamageTextStyle.Normal, $"+{amount}");
			SummonManager.Instance.UpdateManaBar(_currentMana);
		}
	}

	public override void Die()
	{
		base.Die();
		
		IsDeath = true;

		GameManager.Instance.ShowGameOverScreen();

		gameObject.SetActive(false);
	}
}
