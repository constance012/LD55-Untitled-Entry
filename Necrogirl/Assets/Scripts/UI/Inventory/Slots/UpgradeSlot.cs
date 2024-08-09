using System;

public class UpgradeSlot : StaticItemSlot<UpgradeBase>, IUpgradeSlot
{
	public UpgradeBase Current => _current;
	public bool HasItem => _current != null;
	public int Quantity => Convert.ToInt32(_current != null);

	protected override void Start()
	{
		base.Start();
		icon.gameObject.SetActive(false);
	}

	public bool Add(UpgradeBase upgrade)
	{
		bool success = false;

		if (!HasItem)
		{
			_current = upgrade;
			_current.DoUpgrade();

			icon.sprite = _current.icon;
			icon.gameObject.SetActive(true);

			if (tooltipTrigger != null)
			{
				tooltipTrigger.header = _current.displayName;
				tooltipTrigger.content = _current.description;
			}

			success = true;
		}

		UpdateQuantityText();
		return success;
	}

	public void Remove()
	{
		if (HasItem)
		{
			_current.RemoveUpgrade();
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

	public bool Exists(UpgradeBase upgrade)
	{
		if (!HasItem)
			return false;
		
		return _current.displayName.Equals(upgrade.displayName);
	}

	public override void UpdateQuantityText()
	{
		// To be changed if upgrades are stackable.
		quantityText.text = "";
	}
}