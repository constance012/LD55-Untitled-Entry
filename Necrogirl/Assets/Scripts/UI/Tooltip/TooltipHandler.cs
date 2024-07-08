using UnityEngine;

public class TooltipHandler : Singleton<TooltipHandler>
{
	[Header("Tooltip Reference"), Space]
	[SerializeField] private Tooltip tooltip;
	
	private bool _isShowed;

	protected override void Awake()
	{
		base.Awake();
		tooltip = GetComponentInChildren<Tooltip>(true);
	}

	public static void Show(string contentText, string headerText = "")
	{
		if (!Instance._isShowed)
		{
			Instance.tooltip.SetText(contentText, headerText);

			Instance.tooltip.gameObject.SetActive(true);
			Instance._isShowed = true;
		}
	}

	public static void Hide()
	{
		if (Instance._isShowed)
		{
			Instance.tooltip.gameObject.SetActive(false);
			Instance._isShowed = false;
		}
	}
}
