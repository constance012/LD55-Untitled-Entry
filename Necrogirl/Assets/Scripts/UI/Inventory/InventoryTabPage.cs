using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class InventoryTabPage<TSlot, TItem>
					where TSlot : IInventorySlot<TItem>
					where TItem : IdentifiableSO
{
	private List<TSlot> _slots;

	public InventoryTabPage(List<GameObject> slotObjects)
	{
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
		List<StaticItemSlot<TItem>> slots = _slots.ConvertAll(slot => slot as StaticItemSlot<TItem>);

		slots.ForEach(slot => slot.PrepareEffect());

		for (int i = 0; i < tasks.Length; i++)
		{
			tasks[i] = slots[i].PerformEffect();
			await Task.Delay(10);
		}

		await Task.WhenAll(tasks);
	}
	#endregion

	#region Item Management Methods.
	/// <summary>
	/// Add items to inventory slots.
	/// </summary>
	/// <param name="item"></param>
	public bool Add(TItem item)
	{
		int emptySlotIndex = IndexOf();

		// This item is stackable, check for any available stacks.
		int storedIndex = IndexOf(item);

		while (storedIndex != -1)
		{
			if (_slots[storedIndex].Add(item))
				return true;
			
			storedIndex = IndexOf(item);
		}
		
		// This item is either not stackable or all stacks are full, and the Inventory is not full yet, add it to the new slot.
		if (emptySlotIndex != -1)
		{
			return _slots[emptySlotIndex].Add(item);
		}

		return false;
	}

	public void Remove(TItem item)
	{
		int index = IndexOf(item.id);

		if (index != -1)
			_slots[index].Remove();
	}
	#endregion

	#region Utilities Methods.

	/// <summary>
	/// Returns an index of the first empty slot available.
	/// </summary>
	/// <returns></returns>
	private int IndexOf()
	{
		return _slots.FindIndex(slot => !slot.HasItem);
	}

	/// <summary>
	/// Returns an index of the first slot contains the provided ID, otherwise return the first empty slot.
	/// </summary>
	/// <param name="id"> The item's id to search. </param>
	/// <returns></returns>
	private int IndexOf(string id)
	{
		if (!string.IsNullOrEmpty(id))
			return _slots.FindIndex(slot => slot.Current.id.Equals(id));
		
		return IndexOf();
	}

	/// <summary>
	/// Returns an index of the first slot contains the provided item if it exists, otherwise return the first empty slot.
	/// </summary>
	/// <param name="item"> The item to search. </param>
	/// <returns></returns>
	private int IndexOf(TItem item = null)
	{
		if (item != null)
			return _slots.FindIndex(slot => slot.Exists(item));

		return IndexOf();
	}
	#endregion
}