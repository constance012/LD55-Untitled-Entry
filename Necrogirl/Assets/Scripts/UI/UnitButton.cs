using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class UnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI manaCostText;
	[SerializeField] private Image cooldownIndicator;
	
	[Header("References"), Space]
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private Button button;

	[Header("Tooltip Trigger"), Space]
	[SerializeField] private TooltipTrigger trigger;

	// Properties.
	public float ManaCost => _unitStats.GetStaticStat(Stat.ManaCost);

	// Private fields.
	private RectTransform _rectTransform;
	private Stats _unitStats;
	private float _maxCooldown;
	private float _currentCooldown;
	
	private void Start()
	{
		_rectTransform = GetComponent<RectTransform>();
		_unitStats = EntityDatabase.Instance.unitStats[transform.GetSiblingIndex()];
		manaCostText.text = _unitStats.GetStaticStat(Stat.ManaCost).ToString();
		trigger.content = _unitStats.ToString();
	}

	private void Update()
	{
		if (_currentCooldown > -.1f)
		{
			cooldownIndicator.fillAmount = Mathf.InverseLerp(0f, _maxCooldown, _currentCooldown);
			_currentCooldown -= Time.deltaTime;
		}
	}

	public void OnPointerEnter(PointerEventData e)
	{
		_rectTransform.DOLocalMoveY(25f, .1f);
		_rectTransform.DOScale(1.1f, .1f);
	}

	public void OnPointerExit(PointerEventData e)
	{
		_rectTransform.DOLocalMoveY(0f, .1f);
		_rectTransform.DOScale(1f, .1f);
	}

	public void ValidateManaPoint(float currentMana)
	{
		if (_unitStats == null)
			_unitStats = EntityDatabase.Instance.unitStats[transform.GetSiblingIndex()];

		canvasGroup.alpha = currentMana < _unitStats.GetStaticStat(Stat.ManaCost) ? .5f : 1f;
		button.interactable = currentMana >= _unitStats.GetStaticStat(Stat.ManaCost);
	}

	public void SetCooldownTime(float cooldown)
	{
		_maxCooldown = cooldown;
		_currentCooldown = cooldown;
	}

	public void SetInteractable(bool state)
	{
		canvasGroup.alpha = !state ? .5f : 1f;
		button.interactable = state;
	}
}