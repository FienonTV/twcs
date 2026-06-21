using Godot;

[GlobalClass]
public partial class InventoryDataResource : Resource
{
    [Export]
    public SlotDataResource[] Slots;

    public bool AddItem(ItemDataResource item, int amount = 1)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        int remaining = amount;

        foreach (SlotDataResource slot in Slots)
        {
            if (slot != null && slot.ItemData == item && slot.Quantity < item.MaxStackSize)
            {
                int space = item.MaxStackSize - slot.Quantity;
                int transfer = Mathf.Min(space, remaining);
                slot.Quantity += transfer;
                remaining -= transfer;

                if (remaining <= 0)
                {
                    return true;
                }
            }
        }

        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i] == null)
            {
                Slots[i] = new SlotDataResource();
                Slots[i].ItemData = item;
                Slots[i].Quantity = Mathf.Min(remaining, item.MaxStackSize);
                remaining -= Slots[i].Quantity;

                if (remaining <= 0)
                {
                    return true;
                }
            }
        }

        if (remaining < amount)
        {
            Logger.Warning("InventoryDataResource: Inventory is partially full; could not add all items.");
            return false;
        }

        Logger.Warning("InventoryDataResource: Inventory is full.");
        return false;
    }
}
