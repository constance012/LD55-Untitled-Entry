using UnityEngine;

public interface IInventorySlot<TItem> where TItem : ScriptableObject
{
	public TItem Current { get; }
	public bool HasItem { get; }
	public int Quantity { get; }

	public bool Add(TItem item);
	public void Remove();
	public bool Exists(TItem item);
}