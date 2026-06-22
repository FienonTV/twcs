using Godot;

public partial class InventoryUI : Control
{
    private PackedScene _InventorySlotScene;

    [Export]
    public InventoryDataResource _Data;

    private InventoryMenu _InventoryMenu;

    public override void _Ready()
    {
        _InventoryMenu = Services.Get<InventoryMenu>();
        if (_InventoryMenu == null)
        {
            Logger.Error("InventoryUI: InventoryMenu autoload not found.");
            return;
        }

        _InventorySlotScene = ResourceLoader.Load<PackedScene>(ResourcePaths.InventorySlotScene);
        if (_InventorySlotScene == null)
        {
            Logger.Error($"InventoryUI: Failed to load inventory slot scene from '{ResourcePaths.InventorySlotScene}'.");
            return;
        }

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
        if (_InventorySlotScene == null || _Data?.Slots == null)
        {
            return;
        }

        ClearInventory();

        Character currentUser = _InventoryMenu?.CurrentUser;
        foreach (SlotDataResource s in _Data.Slots)
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
