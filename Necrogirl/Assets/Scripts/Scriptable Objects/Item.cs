using System;
using UnityEngine;
using CSTGames.DataPersistence;
using CSTGames.Utility;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Base Item")]
public class Item : ScriptableObject
{
	[Serializable]
	public struct Rarity
	{
		public string title;
		public Color color;
	}

	[ContextMenu("Generate ID")]
	private void GenerateID()
	{
		id = Guid.NewGuid().ToString();
	}

	[ContextMenu("Clear ID")]
	private void ClearID()
	{
		id = "";
	}

	[HideInInspector] public string id;
	
	[Header("General")]
	[Space]

	[ReadOnly] public int slotIndex = -1;
	public ItemCategory category;

	public string itemName;
	[TextArea(5, 10)] public string description;

	public Sprite icon;
	public Rarity rarity;

	[Header("Quantity and Stack")]
	[Space]

	public bool stackable;
	public int maxPerStack = 1;
	public int quantity = 1;

	[Header("Specials")]
	[Space]

	public bool isFavorite;
	public bool isDefaultItem;
	public bool canBeUsed;

	public virtual bool Use(bool forced = false)
	{
		Debug.Log("Using " + itemName);
		return true;
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
		this.itemName = saveData.itemName;
		this.category = saveData.category;

		this.slotIndex = saveData.slotIndex;
		this.quantity = saveData.quantity;
		this.isFavorite = saveData.isFavorite;
	}
}

public enum ItemCategory
{
	Null,
	Consumable,
	Potion,
	Material,
	Coin,
	Special,
	KeyItem
}
