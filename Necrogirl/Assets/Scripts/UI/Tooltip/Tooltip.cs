using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
	[Header("Position Settings"), Space]
	[SerializeField] private Vector2 pivotOffet;

	[Header("References"), Space]
	[Header("Texts")]
	[Space]
	[SerializeField] private TextMeshProUGUI header;
	[SerializeField] private TextMeshProUGUI content;

	[Header("Others")]
	[Space]
	[SerializeField] private LayoutElement layoutElement;
	[SerializeField] private RectTransform rectTransform;

	// Update is called once per frame
	private void Update()
	{
		Vector2 mousePos = Input.mousePosition;
		
		float mouseXRatio = mousePos.x / Screen.width;
		float mouseYRatio = mousePos.y / Screen.height;

		float pivotX = mouseXRatio < .5f ? 0f - pivotOffet.x : 1f + pivotOffet.x;
		float pivotY = mouseYRatio < .5f ? 0f - pivotOffet.y : 1f + pivotOffet.y;

		rectTransform.pivot = new Vector2(pivotX, pivotY);
		transform.position = mousePos;
	}

	public void SetText(string contentText, string headerText = "")
	{
		// Hide the header gameobject if the header text is null or empty.
		if (string.IsNullOrEmpty(headerText))
			header.gameObject.SetActive(false);
		
		else
		{
			header.gameObject.SetActive(true);
			header.text = headerText.ToUpper();
		}

		content.text = contentText.ToUpper();

		// And finally, toggle the Layout Element depending on the width.
		layoutElement.enabled = (header.preferredWidth > layoutElement.preferredWidth ||
									content.preferredWidth > layoutElement.preferredWidth);
	}
}
