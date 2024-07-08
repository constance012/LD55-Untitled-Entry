using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HealthBar : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Slider mainSlider;
	[SerializeField] private Slider fxSlider;

	[Space, SerializeField] private TextMeshProUGUI displayText;

	[Header("Configuration"), Space]
	[SerializeField] private Gradient healthGradient;

	[Header("Effect Settings"), Space]
	[SerializeField] private float fxDelay;
	[SerializeField] private float fxDuration;
	[Space, SerializeField] private Color healthIncreaseColor;
	[SerializeField] private Color healthDecreaseColor;

	// Properties.
	public bool IsPreviousEffectActive => _fxTween.IsActive();

	// Private fields.
	private Image _mainFillRect;
	private Image _fxFillRect;
	private Tween _fxTween;

	protected virtual void Awake()
	{
		_mainFillRect = mainSlider.fillRect.GetComponent<Image>();
		_fxFillRect = fxSlider.fillRect.GetComponent<Image>();
	}

	public void SetCurrentHealth(float current)
	{
		// Health decreasing.
		if (current <= mainSlider.value)
		{
			_fxFillRect.color = healthDecreaseColor;

			mainSlider.value = current;
			_mainFillRect.color = healthGradient.Evaluate(mainSlider.normalizedValue);
		}

		// Health increasing.
		else
		{
			_fxFillRect.color = healthIncreaseColor;
			fxSlider.value = current;
		}

		displayText.text = $"{current:0} / {mainSlider.maxValue:0}";

		if (IsPreviousEffectActive)
			_fxTween.Kill();

		_fxTween = PerformEffect();
	}

	public void SetMaxHealth(float max, bool initialize = true)
	{
		mainSlider.maxValue = max;
		fxSlider.maxValue = max;
	
		if (initialize)
		{
			mainSlider.value = max;
			fxSlider.value = max;
			
			_mainFillRect.color = healthGradient.Evaluate(mainSlider.normalizedValue);
			displayText.text = $"{max:0} / {max:0}";
		}
	}

	private Tween PerformEffect()
	{
		if (_fxFillRect.color == healthIncreaseColor)
		{
			Sequence sequence = DOTween.Sequence();
			
			sequence.Append(mainSlider.DOValue(fxSlider.value, fxDuration).SetEase(Ease.OutCubic))
					.Join(_mainFillRect.DOColor(healthGradient.Evaluate(mainSlider.normalizedValue), fxDuration))
					.SetDelay(fxDelay);

			return sequence;
		}
		else
		{
			return fxSlider.DOValue(mainSlider.value, fxDuration).SetEase(Ease.OutCubic).SetDelay(fxDelay);
		}
	}
}
