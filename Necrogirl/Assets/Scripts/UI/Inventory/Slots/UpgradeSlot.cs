using System;

public class UpgradeSlot : InventorySlotBase, IUpgradeSlot
{
	public UpgradeBase Current => _currentUpgrade;
	public bool HasItem => _currentUpgrade != null;
	public int Quantity => Convert.ToInt32(_currentUpgrade != null);

	// Private fields.
	private UpgradeBase _currentUpgrade;

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
			_currentUpgrade = upgrade;

			icon.sprite = _currentUpgrade.icon;
			icon.gameObject.SetActive(true);

			if (tooltipTrigger != null)
			{
				tooltipTrigger.header = _currentUpgrade.upgradeName;
				tooltipTrigger.content = _currentUpgrade.description;
			}

			_currentUpgrade.DoUpgrade();
			success = true;
		}

		UpdateQuantityText();
		return success;
	}

	public void Remove()
	{
		if (HasItem)
		{
			_currentUpgrade.RemoveUpgrade();
			_currentUpgrade = null;
			
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
		else
			return _currentUpgrade.upgradeName.Equals(upgrade.upgradeName);
	}

	public override void UpdateQuantityText()
	{
		// To be changed if upgrades are stackable.
		quantityText.text = "";
	}
}