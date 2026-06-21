using Godot;
using System;


public partial class InventoryUI : Control
{
    private PackedScene _InventorySlotScene;

    [Export]
    InventoryDataResource _Data;

    inventory_menu _InventoryMenu;

    public override void _Ready()
    {
        _InventoryMenu = Owner as inventory_menu;
        if (_InventoryMenu == null)
        {
            Logger.Error("InventoryUI: Owner is not inventory_menu.");
            return;
        }

        _InventorySlotScene = ResourceLoader.Load<PackedScene>(ResourcePaths.InventorySlotScene);
        if (_InventorySlotScene == null)
        {
            Logger.Error($"InventoryUI: Failed to load inventory slot scene from '{ResourcePaths.InventorySlotScene}'.");
            return;
        }

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
        if (_InventorySlotScene == null || _Data?._Slots == null)
        {
            return;
        }

        ClearInventory();

        Character currentUser = _InventoryMenu?.CurrentUser;
        foreach (SlotDataResource s in _Data._Slots)
        {
            if (s != null)
            {
                s.User = currentUser;
            }

            InventorySlotUI slot = _InventorySlotScene.Instantiate() as InventorySlotUI;
            AddChild(slot);
            slot.SlotData = s;
        }
    }
}
