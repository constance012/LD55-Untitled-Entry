using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
	public enum HealingType
	{
		Health,
		Mana
	}

	[Header("Healing amount"), Space]
	public HealingType healingType;
	public int healingAmount;

	public override bool Use(bool forced = false)
	{
		if (quantity > 0 && canBeUsed)
		{
			PlayerStats player = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();

			if (healingType == HealingType.Health)
			{
				player.Heal(healingAmount);
				quantity--;
				return true;
			}
			else if (player.CurrentMana < player.MaxMana || forced)
			{
				player.RecoverMana(healingAmount);
				return true;
			}
		}
		else
			Debug.LogWarning($"This {itemName} can not be used or its quantity is 0!!");

		return false;
	}

	public override string ToString()
	{
		return base.ToString() + "\n" +
				$"<b> +{healingAmount} HP. </b>\n" +
				$"<b> Right Click to use. </b>";
	}
}
