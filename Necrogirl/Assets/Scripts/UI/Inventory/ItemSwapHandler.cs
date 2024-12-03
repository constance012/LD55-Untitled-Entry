using UnityEngine;

public class ItemSwapHandler : Singleton<ItemSwapHandler>
{
	[Header("Dynamic Slot Prefab"), Space]
	[SerializeField] private GameObject dynamicSlotPrefab;
	[SerializeField] private RectTransform cloneParent;

	// Properties.
	public bool IsStandBy => _registeredSlot == null && _dynamicSlot == null;

	// Private fields.
	private DynamicItemSlot _dynamicSlot;
	private PickableItemSlot _registeredSlot;

	public void Register(PickableItemSlot clicked)
	{
		if (IsStandBy && clicked.HasItem)
		{
			// The player is not holding anything and this slot contains items.
			_registeredSlot = clicked;
			_registeredSlot.SetIconAlpha(.5f);

			GameObject dynamicSlotGO = Instantiate(dynamicSlotPrefab, cloneParent);
			
			if (dynamicSlotGO.TryGetComponent(out _dynamicSlot))
				_dynamicSlot.Add(_registeredSlot.Current);
		}
		else if (_dynamicSlot != null)
		{
			// Something is being held.
			bool readyToDispose = false;
			int originalQuantity = _dynamicSlot.Quantity;

			if (clicked != _registeredSlot)
			{
				// This slot is not the same as the original slot.
				if (!clicked.HasItem)
				{
					// This slot is empty.
					_registeredSlot.Remove();
					clicked.Add(_dynamicSlot.Current);
					readyToDispose = true;
				}
				else
				{
					// This slot contains items.
					if (clicked.Exists(_dynamicSlot.Current))
					{
						// This slot contains the same item type, try stacking them up.
						if (clicked.Add(_dynamicSlot.Current))
						{
							// They stack with no redundance.
							_registeredSlot.Remove();
							readyToDispose = true;
						}
						else if (_dynamicSlot.Quantity != originalQuantity)
						{
							// There's redundance left.
							_dynamicSlot.UpdateQuantityText();
							_registeredSlot.UpdateQuantityText();
							return;
						}
					}

					if (!readyToDispose)
					{
						// This slot contains different item type, or the same type but its stack is full. Then swap their positions.
						_registeredSlot.Remove();
						_registeredSlot.Add(clicked.Current);

						clicked.Remove();
						clicked.Add(_dynamicSlot.Current);

						readyToDispose = true;
					}
				}
			}
			else
			{
				// This slot is the same as the original, do nothing.
				readyToDispose = true;
			}
			
			if (readyToDispose)
			{
				// Dispose resources.
				_registeredSlot.SetIconAlpha(1f);
				_registeredSlot = null;
				Destroy(_dynamicSlot.gameObject);
			}
		}
	}
}