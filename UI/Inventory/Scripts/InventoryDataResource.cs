using Godot;

[GlobalClass]
public partial class InventoryDataResource : Resource
{
	[Export]
	public SlotDataResource[] _Slots;


	public bool addItem(ItemDataResource item, int amount = 1)
	{
		if (item == null || amount <= 0)
		{
			return false;
		}

		int remaining = amount;

		foreach (SlotDataResource slot in _Slots)
		{
			if (slot != null && slot._ItemData == item && slot._Quantity < item._MaxStackSize)
			{
				int space = item._MaxStackSize - slot._Quantity;
				int transfer = Mathf.Min(space, remaining);
				slot._Quantity += transfer;
				remaining -= transfer;

				if (remaining <= 0)
				{
					return true;
				}
			}
		}

		for (int i = 0; i < _Slots.Length; i++)
		{
			if (_Slots[i] == null)
			{
				_Slots[i] = new SlotDataResource();
				_Slots[i]._ItemData = item;
				_Slots[i]._Quantity = Mathf.Min(remaining, item._MaxStackSize);
				remaining -= _Slots[i]._Quantity;

				if (remaining <= 0)
				{
					return true;
				}
			}
		}

		if (remaining < amount)
		{
			GD.Print("Inventory is partially full; could not add all items.");
			return false;
		}

		GD.Print("Inventory is full");
		return false;
	}
}
