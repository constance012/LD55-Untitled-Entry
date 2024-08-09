using System.Threading.Tasks;
using UnityEngine;

public abstract class StaticItemSlot<TItem> : InventorySlotBase<TItem> where TItem : ScriptableObject
{
	[Header("Tooltip Trigger"), Space]
	[SerializeField] protected TooltipTrigger tooltipTrigger;

	[Header("Tweening Effect"), Space]
	[SerializeField] protected TweenableUIElement effect;

	public void PrepareEffect() => effect.SetStartValues();
	public async Task PerformEffect() => await effect.StartTweening(true);
}