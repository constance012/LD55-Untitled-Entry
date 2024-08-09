using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PickableItemSlot : StaticItemSlot<Item>, IItemSlot, IPointerClickHandler
{
	[Header("Slot Type"), Space]
	[SerializeField] private bool isHUD;

	public Item Current => _current;
	public bool HasItem => _current != null;

	public int Quantity
	{
		get
		{
			if (HasItem)
				return _current.quantity;
			return -1;
		}
	}
	
	public int SlotIndex => transform.GetSiblingIndex();

	protected override void Start()
	{
		base.Start();

		if (!HasItem)
			icon.gameObject.SetActive(isHUD);
	}

	public void OnPointerClick(PointerEventData e)
	{
		ItemSwapHandler.Instance.Register(this);
	}

	#region Item Management Methods.
	/// <summary>
	/// Add the item into this slot if this slot is empty, otherwise update its quantity if it's stackable.
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public bool Add(Item item)
	{
		bool success = false;

		if (!HasItem)
		{
			_current = item;
			_current.id = Guid.NewGuid().ToString();
			_current.slotIndex = SlotIndex;

			icon.sprite = _current.icon;
			icon.gameObject.SetActive(true);

			if (tooltipTrigger != null)
			{
				tooltipTrigger.header = _current.displayName;
				tooltipTrigger.content = _current.description;
			}

			success = true;
		}
		else if (_current.stackable)
		{
			int redundance = _current.UpdateQuantity(item.quantity);
			success = redundance <= 0;  // Success if there's no redundance.

			if (redundance > 0)
				item.quantity = redundance;
		}

		UpdateQuantityText();
		return success;
	}

	public void Remove()
	{
		if (HasItem)
		{
			_current = null;
			
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
		if (HasItem && _current.Use())
		{
			UpdateQuantityText();

			if (_current.quantity == 0)
				Inventory.Instance.RemoveItem(_current);
		}
	}

	public bool Exists(Item item)
	{
		if (!HasItem)
			return false;
		
		return _current.displayName.Equals(item.displayName) && !_current.FullyStacked;
	}
	#endregion

	#region UI Control Methods.
	public void SetIconAlpha(float alpha)
	{
		icon.DOFade(alpha, 0f);
	}

	public override void UpdateQuantityText()
	{
		int quantity = Quantity;
		
		if (quantity == -1)
			quantityText.text = isHUD ? "0" : "";
		else
			quantityText.text = quantity.ToString();
	}
	#endregion
}