using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private TextMeshProUGUI manaCostText;
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private Button button;

	[Header("Tooltip"), Space]
	[SerializeField] private TooltipTrigger trigger;

	// Properties.
	public float ManaCost => _unitStats.GetStaticStat(Stat.ManaCost);

	// Private fields.
	private Stats _unitStats;
	
	private void Start()
	{
		_unitStats = EntityDatabase.Instance.unitStats[transform.GetSiblingIndex()];
		manaCostText.text = _unitStats.GetStaticStat(Stat.ManaCost).ToString();
		trigger.content = _unitStats.ToString().ToUpper();
	}

	public void ValidateManaPoint(float currentMana)
	{
		if (_unitStats == null)
			_unitStats = EntityDatabase.Instance.unitStats[transform.GetSiblingIndex()];

		canvasGroup.alpha = currentMana < _unitStats.GetStaticStat(Stat.ManaCost) ? .5f : 1f;
		button.interactable = currentMana >= _unitStats.GetStaticStat(Stat.ManaCost);
	}

	public void SetInteractable(bool state)
	{
		canvasGroup.alpha = !state ? .5f : 1f;
		button.interactable = state;
	}
}