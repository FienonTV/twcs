using Godot;
using System;


public partial class InventoryUI : Control
{
    static readonly PackedScene InventorySlot = ResourceLoader.Load<PackedScene>(ResourcePaths.InventorySlotScene);

    [Export]
    InventoryDataResource _Data;

    inventory_menu _InventoryMenu;

    public override void _Ready()
    {
        _InventoryMenu = Owner as inventory_menu;
        _InventoryMenu.InventoryActive += UpdateInventory;
        _InventoryMenu.InventoryHidden += ClearInventory;
        ClearInventory();
    }

    public void ClearInventory()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
    }

    public void UpdateInventory()
    {
        Character currentUser = _InventoryMenu?.CurrentUser;
        foreach (SlotDataResource s in _Data._Slots)
        {
            if (s != null)
            {
                s.User = currentUser;
            }

            InventorySlotUI slot = InventorySlot.Instantiate() as InventorySlotUI;
            AddChild(slot);
            slot.SlotData = s;
        }
    }
}
