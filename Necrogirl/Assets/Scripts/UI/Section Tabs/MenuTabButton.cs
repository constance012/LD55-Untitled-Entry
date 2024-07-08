using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Graphic))]
public class MenuTabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	[Header("Current Tab Group"), Space]
	[SerializeField] private TabGroup tabGroup;
	[SerializeField] private bool selectOnStartup;

	[Header("Content Page"), Space]
	public GameObject contentPage;

	[Header("Effect"), Space]
	[SerializeField] private float colorFadeTime;

	[Header("Event"), Space]
	public UnityEvent onTabSelected;

	// Private fields.
	private Graphic _graphic;

	private void Awake()
	{
		_graphic = GetComponent<Graphic>();	
		tabGroup.Subscribe(this);
	}

	private void Start()
	{
		if (selectOnStartup)
			OnPointerClick(null);
	}

	#region Interface Methods.
	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("Tab Entered");
		tabGroup.OnTabEnter(this);
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("Tab Exited");
		tabGroup.OnTabExit();
	}
	
	public void OnPointerClick(PointerEventData eventData)
	{
		if (tabGroup.TrySelectTab(this))
			onTabSelected?.Invoke();
	}
	#endregion

	public void SetContentState(bool state)
	{
		if (contentPage != null)
			contentPage.SetActive(state);
	}

	public void SetGraphicColor(Color targetColor)
	{
		_graphic.DOColor(targetColor, colorFadeTime)
				.SetEase(Ease.OutCubic)
				.SetUpdate(UpdateType.Normal, true);
	}
}
