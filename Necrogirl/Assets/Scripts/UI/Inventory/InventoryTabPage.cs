using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class InventoryTabPage<TSlot, TItem>
					where TSlot : IInventorySlot<TItem>
					where TItem : ScriptableObject
{
	private List<TSlot> _slots;
	private int _slotLastIndex = 0;
	private bool _stackableItems;

	public InventoryTabPage(List<GameObject> slotObjects, bool stackable)
	{
		_stackableItems = stackable;
		_slots ??= new List<TSlot>();
		
		for (int i = 0; i < slotObjects.Count; i++)
		{
			if (slotObjects[i].TryGetComponent(out TSlot component))
				_slots.Add(component);
		}
	}

	#region Slots' Startup Animation Handler.
	public async void HandleSlotsAnimation()
	{
		Task[] tasks = new Task[_slots.Count];
		List<InventorySlotBase> slots = _slots.ConvertAll(slot => slot as InventorySlotBase);

		slots.ForEach(slot => slot.PrepareEffect());

		for (int i = 0; i < tasks.Length; i++)
		{
			tasks[i] = slots[i].PerformEffect();
			await Task.Delay(10);
		}

		await Task.WhenAll(tasks);
	}
	#endregion

	#region Item Management.
	/// <summary>
	/// Add items to inventory slots.
	/// </summary>
	/// <param name="item"></param>
	public void Add(TItem item)
	{
		if (_stackableItems)
		{
			int storedIndex = _slots.FindIndex(0, _slotLastIndex + 1, slot => slot.Exists(item));

			if (storedIndex == -1)
				_slots[_slotLastIndex++].Add(item);
			else
				_slots[storedIndex].Add(item);
		}
		else
			_slots[_slotLastIndex++].Add(item);
	}

	public void Remove(TItem item)
	{
		int index = _slots.FindIndex(0, _slotLastIndex + 1, slot => slot.Current == item);

		// Sort the array down if the provided index is less than the currently tracked index.
		if (index != -1)
		{
			if (index < _slotLastIndex - 1)
			{
				_slots[index].Remove();

				for (int i = index; i < _slotLastIndex; i++)
					_slots[i].Add(_slots[i + 1].Current);
			}

			_slots[--_slotLastIndex].Remove();
		}
	}
	#endregion
}