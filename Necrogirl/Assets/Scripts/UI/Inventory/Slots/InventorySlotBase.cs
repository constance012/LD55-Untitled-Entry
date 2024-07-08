using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class InventorySlotBase : MonoBehaviour
{
	[Header("UI References"), Space]
	[SerializeField] protected Image icon;
	[SerializeField] protected TextMeshProUGUI quantityText;

	[Header("Tooltip Trigger"), Space]
	[SerializeField] protected TooltipTrigger tooltipTrigger;

	[Header("Tweening Effect"), Space]
	[SerializeField] protected TweenableUIElement effect;

	protected virtual void Start()
	{
		UpdateQuantityText();
	}

	public virtual void UpdateQuantityText() {}

	public void PrepareEffect() => effect.SetStartValues();
	public async Task PerformEffect() => await effect.StartTweening(true);
}