using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public enum DamageTextStyle { Small, Normal, Critical }

/// <summary>
/// A class to generates an UI popup text.
/// </summary
public class DamageText : MonoBehaviour
{
	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI displayText;
	[SerializeField] private CanvasGroup canvasGroup;
	
	[Header("Configurations"), Space]
	[Min(1f)] public float maxLifeTime = 1f;
	[Space, Min(.5f)] public float maxVelocity;
	
	// Color Constants.
	public static readonly Color CriticalColor = new Color(.821f, .546f, .159f);
	public static readonly Color DamageColor = Color.red;
	public static readonly Color HealingColor = Color.green;
	public static readonly Color ManaColor = new Color(.129f, .509f, .830f);
	public static readonly Color BonusScoreColor = new Color(.792f, .13f, .714f);

	// Style Scales.
	private static readonly Dictionary<DamageTextStyle, float> _styleScales = new Dictionary<DamageTextStyle, float>()
	{
		[DamageTextStyle.Small] = .15f,
		[DamageTextStyle.Normal] = .2f,
		[DamageTextStyle.Critical] = .25f
	};

	// Private fields.
	private Color _currentTextColor;
	private bool _criticalHit;

	#region Generate Method Overloads.
	// Default color is red, and parent is world canvas.
	public static DamageText Generate(GameObject prefab, Vector3 pos, DamageTextStyle style, string textContent)
	{
		Transform canvas = GameObject.FindWithTag("WorldCanvas").transform;
		GameObject dmgTextObj = Instantiate(prefab, pos, Quaternion.identity);
		dmgTextObj.transform.SetParent(canvas, true);

		DamageText dmgText = dmgTextObj.GetComponentInChildren<DamageText>();

		Color textColor = style == DamageTextStyle.Critical ? CriticalColor : DamageColor;

		dmgText.Setup(textColor, textContent, style);
		return dmgText;
	}

	// Default parent is world canvas.
	public static DamageText Generate(GameObject prefab, Vector3 pos, Color txtColor, DamageTextStyle style, string textContent)
	{
		Transform canvas = GameObject.FindWithTag("WorldCanvas").transform;

		GameObject dmgTextObj = Instantiate(prefab, pos, Quaternion.identity);
		dmgTextObj.transform.SetParent(canvas, true);

		DamageText dmgText = dmgTextObj.GetComponentInChildren<DamageText>();

		dmgText.Setup(txtColor, textContent, style);
		return dmgText;
	}
	#endregion

	private void Setup(Color txtColor, string textContent, DamageTextStyle style)
	{
		transform.localScale = Vector3.zero;
		_currentTextColor = txtColor;
		_criticalHit = style == DamageTextStyle.Critical;

		displayText.text = textContent.ToUpper();
		displayText.color = _currentTextColor;
		displayText.fontSize = 1f;

		PopUp(style);
		GraduallyMoveUp();
	}

	#region Control Methods.
	private void GraduallyMoveUp()
	{
		float vel = _criticalHit ? maxVelocity * 1.5f : maxVelocity;
		transform.DOLocalMoveY(1f, vel).SetSpeedBased(true).SetEase(Ease.OutQuint).OnComplete(DestroyObject);
	}

	private void PopUp(DamageTextStyle style)
	{
		transform.DOScale(_styleScales[style], .25f).SetEase(Ease.OutBack);

		if (_criticalHit)
			transform.DOScale(.3f, .17f).SetLoops(-1, LoopType.Yoyo);
	}

	public void DestroyObject()
	{
		Sequence sequence = DOTween.Sequence();

		sequence.Append(canvasGroup.DOFade(0f, .15f))
				.Join(transform.DOScale(0f, .2f))
				.SetEase(Ease.OutCubic)
				.AppendCallback(() => Destroy(transform.parent.gameObject));
	}
	#endregion
}
