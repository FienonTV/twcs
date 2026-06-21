using Godot;

[GlobalClass]
public partial class InventoryDataResource : Resource
{
	[Export]
	public SlotDataResource[] _Slots;


	public bool addItem(ItemDataResource item, int amount = 1)
	{
		foreach (SlotDataResource slot in _Slots)
		{
			if (slot != null && slot._ItemData == item)
			{
				slot._Quantity += amount;
				return true;
			}
		}
		for (int i = 0; i < _Slots.Length; i++)
		{
			if (_Slots[i] == null)
			{
				_Slots[i] = new SlotDataResource();
				_Slots[i]._ItemData = item;
				_Slots[i]._Quantity = amount;

				return true;
			}
		}
		GD.Print("Inventory is full");
		return false;
	}
}
