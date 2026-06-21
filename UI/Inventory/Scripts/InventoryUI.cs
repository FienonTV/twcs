using Godot;
using System;


public partial class InventoryUI : Control
{
    static readonly PackedScene INVENTORY_SLOT = ResourceLoader.Load<PackedScene>("res://UI/Inventory/inventory_slot.tscn");

    [Export]
    InventoryDataResource _Data;

    inventory_menu _InventoryMenu;

    public override void _Ready()
    {
        _InventoryMenu = Owner as inventory_menu;
        _InventoryMenu.InventoryActive += updateInventory;
        _InventoryMenu.InventoryHidden += clearInventory;
        clearInventory();


    }

    public void clearInventory()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
    }

    public void updateInventory()
    {
        foreach (SlotDataResource s in _Data._Slots)
        {
            InventorySlotUI slot = INVENTORY_SLOT.Instantiate() as InventorySlotUI;
            AddChild(slot);
            slot.SlotData = s;




        }


    }
}

