public interface IItemSlot : IInventorySlot<Item>
{
	public int SlotIndex { get; }
	public void UseItem();
}