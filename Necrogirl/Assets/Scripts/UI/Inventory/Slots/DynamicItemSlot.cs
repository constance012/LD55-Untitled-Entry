using UnityEngine;

public class DynamicItemSlot : InventorySlotBase<Item>, IDynamicSlot
{
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

	private void Update()
	{
		transform.position = Input.mousePosition;
	}

	public bool Add(Item item)
	{
		if (!HasItem)
		{
			_current = item;
			icon.sprite = _current.icon;

			UpdateQuantityText();
			return true;
		}

		return false;
	}

	public bool Exists(Item item)
	{
		if (!HasItem)
			return false;
		else
			return _current.id.Equals(item.id);
	}

	public void Remove()
	{
		if (HasItem)
		{
			_current = null;
		}
	}

	public override void UpdateQuantityText()
	{
		quantityText.text = Quantity.ToString();
	}
}