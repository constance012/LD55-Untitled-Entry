using System;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

public sealed class UpgradeShop : MonoBehaviour
{
	[Header("Shop Item Slots"), Space]
	[SerializeField] private List<ShopStockSlot> itemSlots = new List<ShopStockSlot>();

	[Header("Upgrades in Stock"), Space]
	[SerializeField] private List<UpgradeBase> stocks = new List<UpgradeBase>();
	[SerializeField, Min(1), Tooltip("The reroll limit, with extra 1 time when the shop is first loaded.")]
	private int rerollLimit;

	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI coinsText;
	[SerializeField] private TextMeshProUGUI confirmText;
	[SerializeField] private TextMeshProUGUI rerollText;

	// Private fields.
	private HashSet<int> _rerollIndices = new HashSet<int>();
	private int _coins;

	private void Start()
	{
		confirmText.text = "Skip";
		rerollText.text = $"Reroll\n<size=-15><color=#DD9E3C>{rerollLimit} times left";

		_coins = Convert.ToInt32(coinsText.text);

		Reroll();
	}

	public async void Reroll()
	{
		if (rerollLimit > 0)
		{
			Button rerollButton = rerollText.GetComponentInParent<Button>();
			_rerollIndices.Clear();

			itemSlots.ForEach(slot => slot.PrepareForReroll());
			
			rerollText.text = "Rerolling...";
			rerollButton.interactable = false;

			for (int i = 0; i < itemSlots.Count; i++)
			{
				ShopStockSlot slot = itemSlots[i];

				// Lock the slot if the remaining stocks is not enough to be added in.
				if (i >= stocks.Count)
				{
					slot.LockSlot();
					continue;
				}

				// Make sure not to reroll to the same upgrade twice.
				int index = UnityRandom.Range(0, stocks.Count);
				while (_rerollIndices.Contains(index))
				{
					index = UnityRandom.Range(0, stocks.Count);
				}

				_rerollIndices.Add(index);
				await slot.AddStock(stocks[index]);
			}

			if (--rerollLimit > 0)
			{
				rerollText.text = $"Reroll\n<size=-15><color=#DD9E3C>{rerollLimit} times left";
				rerollButton.interactable = true;
			}
			else
			{
				rerollText.text = $"Reroll\n<size=-15><color=#DD9E3C>limit reached";
				rerollButton.interactable = false;
			}
		}
	}

	// Callback method for the buy upgrade button.
	public void TryPurchaseUpgrade(ShopStockSlot slot)
	{
		if (slot.TryMakePurchase(_coins))
		{
			_coins -= slot.CurrentUpgrade.goldCost;
			confirmText.text = "Confirm";

			itemSlots.ForEach(slot => slot.CheckForCoinSufficiency(_coins));
			
			if (itemSlots.TrueForAll(slot => slot.IsLocked))
				rerollText.transform.parent.gameObject.SetActive(false);
		}

		coinsText.text = _coins.ToString();
	}
}