using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inventory : Singleton<Inventory>
{
    [Header("HUD Slots"), Space]
    [SerializeField] private GameObject healthPotion;
	[SerializeField] private GameObject coins;

	[Header("Inventory Slots"), Space]
	[SerializeField] private List<GameObject> upgradeSlots = new List<GameObject>();
	[SerializeField] private List<GameObject> itemSlots = new List<GameObject>();

	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI coinsText;

	[Header("Tweening Effect"), Space]
	[SerializeField] private TweenableUIElement tweenable;

	// Properties.
	public int Coins => _coinsHUD.Quantity;

	// Private fields.
	private bool _isActive;
	private IItemSlot _healthPotionHUD;
	private IItemSlot _coinsHUD;
	private InventoryTabPage<IItemSlot, Item> _itemPage;
	private InventoryTabPage<IUpgradeSlot, UpgradeBase> _upgradePage;

	protected override void Awake()
	{
		base.Awake();
		FetchInterfaceComponents();
	}

    private void Update()
    {
		if (SceneManager.GetActiveScene().buildIndex == 1 && !PauseMenu.IsPaused)
		{
			if (InputManager.Instance.GetKeyDown(KeybindingActions.Heal))
				_healthPotionHUD.UseItem();
			
			if (InputManager.Instance.GetKeyDown(KeybindingActions.Inventory))
				ToggleActive(!_isActive);
		}
    }

	public void ToggleActive(bool state)
	{
		_isActive = state;
		Time.timeScale = _isActive ? 0f : 1f;

		tweenable.SetActive(_isActive);
	}

	#region Slots' Startup Animation.
	public void AnimateSlots(string tabName)
	{
		if (Enum.TryParse(tabName, out InventoryTabs tab))
		{
			switch (tab)
			{
				case InventoryTabs.Upgrades:
					_upgradePage.HandleSlotsAnimation();
					break;
				
				case InventoryTabs.Items:
					_itemPage.HandleSlotsAnimation();
					break;
			}
		}
	}
	#endregion


	#region Items and Upgrades Management.
	/// <summary>
	/// Add items to the HUD slots.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="isForcedPickup"></param>
	/// <returns></returns>
    public bool AddItem(Item item, bool isForcedPickup = false)
	{
		if (item.autoUse)
			return item.Use(isForcedPickup);

		switch (item.itemName)
		{
			case "Health Potion":
				AddItem(item);
				return _healthPotionHUD.Add(item);

			case "Coin":
				bool success = _coinsHUD.Add(item);
				if (success)
					coinsText.text = _coinsHUD.Quantity.ToString();
				return success;

			default:
				return false;
		}
	}

	/// <summary>
	/// Add items to inventory slots.
	/// </summary>
	/// <param name="item"></param>
	public void AddItem(Item item)
	{
		_itemPage.Add(item);
	}

	public void RemoveItem(Item item)
	{
		_itemPage.Remove(item);
	}

	public void AddUpgrade(UpgradeBase upgrade)
	{
		_upgradePage.Add(upgrade);
	}

	public void RemoveUpgrade(UpgradeBase upgrade)
	{
		_upgradePage.Remove(upgrade);
	}
	#endregion

	private void FetchInterfaceComponents()
	{
		try
		{
			healthPotion.TryGetComponent(out _healthPotionHUD);
			coins.TryGetComponent(out _coinsHUD);

			_itemPage = new InventoryTabPage<IItemSlot, Item>(itemSlots, stackable: true);
			_upgradePage = new InventoryTabPage<IUpgradeSlot, UpgradeBase>(upgradeSlots, stackable: false);

			coinsText.text = "0";
		}
		catch (Exception)
		{
			Debug.LogWarning("Some game objects of the Inventory script are null or unassigned, ignore this warning if intentional.");
		}
	}

	public enum InventoryTabs
	{
		Upgrades,
		Items
	}
}