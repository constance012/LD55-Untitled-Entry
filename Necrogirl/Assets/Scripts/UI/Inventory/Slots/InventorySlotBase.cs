using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class InventorySlotBase<TItem> : MonoBehaviour where TItem : ScriptableObject
{
	[Header("UI References"), Space]
	[SerializeField] protected Image icon;
	[SerializeField] protected TextMeshProUGUI quantityText;

	// Protected fields.
	protected TItem _current;

	protected virtual void Start()
	{
		UpdateQuantityText();
	}

	public abstract void UpdateQuantityText();
}