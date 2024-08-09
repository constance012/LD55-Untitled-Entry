using UnityEngine;
using CSTGames.DataPersistence;
using CSTGames.Utility;

[CreateAssetMenu(menuName = "Inventory/Base Item", fileName = "New Item")]
public class Item : IdentifiableSO
{
	[Header("Category"), Space]
	[ReadOnly] public int slotIndex = -1;
	public ItemCategory category;

	[Header("Quantity and Stack"), Space]
	public bool stackable;
	public int maxPerStack = 1;
	public int quantity = 1;

	[Header("Specials"), Space]
	public bool canBeUsed;
	public bool autoUse;

	public bool FullyStacked => quantity == maxPerStack;

	/// <summary>
	/// Update the quantity of this item, returns an indicator of redundance as a signed interger.
	/// </summary>
	/// <param name="delta"></param>
	/// <returns> Negative if there's no redundance, 0 if enough to fill the stack, and positive if exceeds the stack.</returns>
	public int UpdateQuantity(int delta)
	{
		int unclamp = quantity + delta;
		quantity = Mathf.Clamp(unclamp, 0, maxPerStack);
		return unclamp - maxPerStack;
	}

	public virtual bool Use(bool forced = false)
	{
		Debug.Log("Using " + displayName);
		return canBeUsed;
	}

	public override string ToString()
	{
		return $"Rarity: <b><color=#{ColorUtility.ToHtmlStringRGB(rarity.color)}> {rarity.title} </color></b>\n" +
				$"Category: <b> {category.ToString().AddWhitespaceBeforeCapital()} </b>\n" +
				$"{description}";
	}

	public virtual void InitializeSaveData(ItemSaveData saveData)
	{
		this.id = saveData.id;
		this.displayName = saveData.itemName;
		this.category = saveData.category;

		this.slotIndex = saveData.slotIndex;
		this.quantity = saveData.quantity;
	}
}

public enum ItemCategory
{
	Null = 0,
	Consumable = 2,
	Material = 3,
	Coin = 4,
	KeyItem = 1
}
