using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SummonManager : Singleton<SummonManager>
{
	[Header("References"), Space]
	[SerializeField] private PlayerStats player;
	[SerializeField] private HealthBar playerManaBar;
	[SerializeField] private List<UnitButton> unitButtons;
	[SerializeField] private Transform container;
	
	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI unitCountText;

	[Header("Summon Settings"), Space]
	[SerializeField] private LayerMask terrainLayers;
	[SerializeField, Min(0f)] private float cooldown;
	[SerializeField] private int maxUnit;
	[SerializeField] private float summonRadius;

	// Private fields.
	private float _cooldown = 0f;
	private bool _maxUnitReached;

	private readonly KeyCode[] summonKeys = new KeyCode[]
	{
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
	};

	private void Update()
	{
		if (GameManager.Instance.GameFinished)
			return;

		_cooldown -= Time.deltaTime;

		if (container.childCount >= maxUnit)
		{
			unitButtons.ForEach(button => button.SetInteractable(false));
			_maxUnitReached = true;
			return;
		}
		else if (container.childCount < maxUnit && _maxUnitReached)
		{
			unitButtons.ForEach(button => button.SetInteractable(true));
			_maxUnitReached = false;
		}

		if (Input.anyKeyDown && _cooldown <= 0f)
		{
			for (int i = 0; i < summonKeys.Length; i++)
			{
				if (Input.GetKeyDown(summonKeys[i]))
				{
					SummonUnit(i);
					break;
				}
			}
		}
	}

	private void LateUpdate()
	{
		unitCountText.text = $"{container.childCount} / {maxUnit}";
	}

	public void SummonUnit(int index)
	{
		float manaCost = EntityDatabase.Instance.unitStats[index].GetStaticStat(Stat.ManaCost);

		if (player.CurrentMana >= manaCost)
		{
			int prefabIndex = index == 0 ? Random.Range(0, 2) : index + 1;
			Vector2 pos = ChooseSummonPosition();

			EntityDatabase.Instance.TryGetEntityByIndex(EntityType.Unit, prefabIndex, out GameObject prefab);
			GameObject unit = Instantiate(prefab, new Vector2(pos.x, pos.y), Quaternion.identity);
			unit.name = prefab.name;
			unit.transform.SetParent(container);

			player.ConsumeMana(manaCost);

			_cooldown = cooldown;
		}
	}

	// Callback method for summoning UI buttons.
	public void SummonUnit(UnitButton button)
	{
		int index = button.transform.GetSiblingIndex();

		if (player.CurrentMana >= button.ManaCost)
		{
			int prefabIndex = index == 0 ? Random.Range(0, 2) : index + 1;
			Vector2 pos = ChooseSummonPosition();

			EntityDatabase.Instance.TryGetEntityByIndex(EntityType.Unit, prefabIndex, out GameObject prefab);
			GameObject unit = Instantiate(prefab, new Vector2(pos.x, pos.y), Quaternion.identity);
			unit.name = prefab.name;
			unit.transform.SetParent(container);

			player.ConsumeMana(button.ManaCost);
			
			_cooldown = cooldown;
		}
	}

	public void UpdateCurrentMana(float currentMana)
	{
		playerManaBar.SetCurrentHealth(currentMana);

		if (!_maxUnitReached)
			unitButtons.ForEach(button => button.ValidateManaPoint(currentMana));
	}

	public void InitializeManaBar(float initialMana)
	{
		playerManaBar.SetMaxHealth(initialMana);

		if (!_maxUnitReached)
			unitButtons.ForEach(button => button.ValidateManaPoint(initialMana));
	}

	private Vector2 ChooseSummonPosition()
	{
		bool validPos = false;
		Vector2 pos = PlayerMovement.Position;

		while (!validPos)
		{
			pos = PlayerMovement.Position + Random.insideUnitCircle * summonRadius;
			validPos = Physics2D.OverlapPoint(pos, terrainLayers) == null;
		}

		return pos;

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(PlayerMovement.Position, summonRadius);
	}
}