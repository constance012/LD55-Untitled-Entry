using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public sealed class ShopStockSlot : MonoBehaviour
{
	[Header("Upgrade in Stock"), Space]
	[SerializeField, ReadOnly] private UpgradeBase currentUpgrade;

	[Header("Tweening Effects"), Space]
	[SerializeField] private TweenableUIElement rerollEffect;

	[Header("UI References"), Space]
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private Image background;
	[SerializeField] private Image icon;
	[SerializeField] private Image tier;
	[SerializeField] private TextMeshProUGUI displayNameText;
	[SerializeField] private TextMeshProUGUI descriptionText;
	[SerializeField] private Button buyButton;

	[Header("Asset References"), Space]
	[SerializeField] private Sprite lockedSprite;

	public UpgradeBase CurrentUpgrade => currentUpgrade;
	public bool IsLocked { get; private set; }

	// Private fields.
	private const string INSUFFICIENT_HIGHLIGHT_COLOR = "#C51B0C";

	public void PrepareForReroll()
	{
		rerollEffect.SetStartValues();
	}

	public async Task AddStock(UpgradeBase upgrade)
	{
		if (currentUpgrade != upgrade || currentUpgrade == null)
		{
			currentUpgrade = upgrade;

			displayNameText.text = currentUpgrade.displayName;
			displayNameText.color = currentUpgrade.rarity.color;
			descriptionText.text = currentUpgrade.description;
			
			icon.sprite = currentUpgrade.icon;
			tier.sprite = currentUpgrade.rarity.icon;

			buyButton.GetComponentInChildren<TextMeshProUGUI>().text = currentUpgrade.goldCost.ToString();
		}
		
		await rerollEffect.StartTweening(true);
		
		if (IsLocked)
			canvasGroup.DOFade(.4f, .2f).SetEase(Ease.OutCubic);
	}

	public void LockSlot()
	{
		if (currentUpgrade == null)
		{
			icon.gameObject.SetActive(false);
			tier.gameObject.SetActive(false);
			displayNameText.gameObject.SetActive(false);
		}

		background.sprite = lockedSprite;
		canvasGroup.alpha = .4f;

		descriptionText.GetComponentInParent<ScrollRect>(true).gameObject.SetActive(false);
		buyButton.gameObject.SetActive(false);

		IsLocked = true;
	}

	public bool TryMakePurchase(int coins)
	{
		if (coins >= currentUpgrade.goldCost)
		{
			Debug.Log($"You've purchased {currentUpgrade.displayName}.");
			Inventory.Instance.AddUpgrade(currentUpgrade);
			LockSlot();

			return true;
		}
		else
		{
			Debug.Log($"Insufficient coins to purchase {currentUpgrade.displayName}.");
			return false;
		}
	}

	public void CheckForCoinSufficiency(int coins)
	{
		if (!IsLocked && coins < currentUpgrade.goldCost)
		{
			ColorBlock cb = buyButton.colors;
			ColorUtility.TryParseHtmlString(INSUFFICIENT_HIGHLIGHT_COLOR, out Color value);
			cb.highlightedColor = value;
				
			buyButton.colors = cb;
		}
	}
}