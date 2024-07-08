using System;
using UnityEngine;

public class PickableItemSlot : InventorySlotBase, IItemSlot
{
	[Header("Slot Type"), Space]
	[SerializeField] private bool isHUD;

	public Item Current => _currentItem;
	public bool HasItem => _currentItem != null;

	public int Quantity
	{
		get
		{
			if (HasItem)
				return _currentItem.quantity;
			return 0;
		}
	}
	
	public int SlotIndex => transform.GetSiblingIndex();


	// Private fields.
	private Item _currentItem;

	protected override void Start()
	{
		base.Start();

		if (!HasItem)
			icon.gameObject.SetActive(isHUD);
	}

	/// <summary>
	/// Add the item into this slot if this slot is empty, otherwise update its quantity.
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public bool Add(Item item)
	{
		bool success;

		if (!HasItem)
		{
			_currentItem = item;
			_currentItem.id = Guid.NewGuid().ToString();
			_currentItem.slotIndex = SlotIndex;

			icon.sprite = _currentItem.icon;
			icon.gameObject.SetActive(true);
			success = true;
		}
		else
		{
			success = _currentItem.UpdateQuantity(item.quantity);
		}

		UpdateQuantityText();
		return success;
	}

	public void Remove()
	{
		if (HasItem)
		{
			_currentItem = null;
			
			icon.gameObject.SetActive(false);

			if (tooltipTrigger != null)
			{
				tooltipTrigger.header = "";
				tooltipTrigger.content = "";
			}
		}

		UpdateQuantityText();
	}

	// Callback method for the UI button.
	public void UseItem()
	{
		if (HasItem && _currentItem.Use())
		{
			UpdateQuantityText();

			if (_currentItem.quantity == 0)
				Inventory.Instance.RemoveItem(_currentItem);
		}
	}

	public bool Exists(Item item)
	{
		if (!HasItem)
			return false;
		else
			return _currentItem.itemName.Equals(item.itemName);
	}

	public override void UpdateQuantityText()
	{
		quantityText.text = HasItem ? _currentItem.quantity.ToString() : isHUD ? "0" : "";
	}
}