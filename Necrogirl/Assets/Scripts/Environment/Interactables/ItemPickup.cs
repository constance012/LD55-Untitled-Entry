using UnityEngine;
using DG.Tweening;

public class ItemPickup : Interactable
{
	[Header("Current Item Info"), Space]
	[Tooltip("The scriptable object represents this item.")]
	public Item itemSO;

	[Header("References"), Space]
	[SerializeField] private GameObject damageTextPrefab;
	[SerializeField] private Rigidbody2D rb2D;

	[Header("Fly Settings"), Space]
	[SerializeField] protected float flyDistance;
	[SerializeField] private float flySpeed;
	[SerializeField] private float pickUpMinDistance;
	[SerializeField] private float pickUpFailDelay;

	public int ItemQuantity
	{
		get { return _currentItem.quantity; }
		set { _overrideQuantity = value; }
	}

	// Private fields.
	private Item _currentItem;
	private int _overrideQuantity = -1;
	private bool _pickedUp;
	private float _delay;

	private void Start()
	{
		_currentItem = Instantiate(itemSO);
		_currentItem.name = itemSO.name;

		if (_overrideQuantity != -1)
			_currentItem.quantity = _overrideQuantity;

		_spriteRenderer.sprite = _currentItem.icon;		
	}

	protected override void CheckForInteraction(float mouseDistance, float playerDistance)
	{
		if (playerDistance <= flyDistance)
			FlyTowardsPlayer();

		base.CheckForInteraction(mouseDistance, playerDistance);
	}

    protected override void TriggerInteraction(float playerDistance)
    {
        base.TriggerInteraction(playerDistance);

		if (LegacyInputManager.Instance.GetKeyDown(KeybindingActions.Interact) && _delay > 0f)
			TryPickup(true);
    }

    private void FlyTowardsPlayer()
	{
		_delay -= Time.deltaTime;

		if (_delay <= 0f)
		{
			Vector2 flyDirection = player.position - transform.position;
			rb2D.velocity = flyDirection.normalized * flySpeed;

			if (flyDirection.sqrMagnitude <= Mathf.Pow(pickUpMinDistance, 2))
				TryPickup();
		}
	}

	protected override void CreatePopupLabel()
	{
		Transform foundLabel = _worldCanvas.transform.Find("Popup Label");

		string itemName = _currentItem.displayName;
		int quantity = _currentItem.quantity;
		Color textColor = _currentItem.rarity.color;

		// Create a clone if not already exists.
		if (foundLabel == null)
		{
			base.CreatePopupLabel();
			_popupLabel.SetLabelName(itemName, quantity, textColor);
		}

		// Otherwise, append to the existing one.
		else
		{
			_popupLabel = foundLabel.GetComponent<InteractionPopupLabel>();

			_popupLabel.SetLabelName(itemName, quantity, textColor, true);
			_popupLabel.RestartAnimation();
		}
	}

	private void TryPickup(bool forced = false)
	{
		if (!_pickedUp)
		{
			if (Inventory.Instance.AddItem(_currentItem, forced))
			{
				Debug.Log("You're picking up a(n) " + _currentItem.displayName);

				if (!_currentItem.autoUse)
				{
					string content = $"{_currentItem.displayName} +{_currentItem.quantity}";
					DamageText.Generate(damageTextPrefab, transform.position + Vector3.up, _currentItem.rarity.color, DamageTextStyle.Normal, content);
				}
				
				_pickedUp = true;
				
				transform.DOScale(0f, .2f).OnComplete(() => {
					if (_popupLabel != null)
						Destroy(_popupLabel.gameObject);

					Destroy(gameObject);
				});
			}
			else
			{
				_delay = pickUpFailDelay;
			}
		}
	}

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, flyDistance);
    }
}
