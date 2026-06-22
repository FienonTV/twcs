using Godot;

public partial class InventorySlotUI : Button
{
    private SlotDataResource _SlotData;

    public SlotDataResource SlotData
    {
        get { return _SlotData; }
        set { SetSlotData(value); }
    }

    [Export]
    private TextureRect _TextureRect;

    [Export]
    private Label _QuantityLabel;

    private InventoryMenu _InventoryMenu;

    public override void _Ready()
    {
        _InventoryMenu = Services.Get<InventoryMenu>();

        if (_TextureRect != null)
        {
            _TextureRect.Texture = null;
        }
        if (_QuantityLabel != null)
        {
            _QuantityLabel.Text = "";
        }

        FocusEntered += ItemFocused;
        FocusExited += ItemUnfocused;
        Pressed += ItemPressed;
    }

    private void SetSlotData(SlotDataResource slotData)
    {
        _SlotData = slotData;

        if (_SlotData == null || _SlotData.ItemData == null || _SlotData.Quantity <= 0)
        {
            if (_TextureRect != null)
            {
                _TextureRect.Texture = null;
            }
            if (_QuantityLabel != null)
            {
                _QuantityLabel.Text = "";
            }
            return;
        }

        if (_TextureRect != null)
        {
            _TextureRect.Texture = _SlotData.ItemData.Texture;
        }
        if (_QuantityLabel != null)
        {
            _QuantityLabel.Text = _SlotData.Quantity.ToString();
        }
    }

    private void ItemFocused()
    {
        if (_SlotData != null && _SlotData.ItemData != null && _InventoryMenu != null)
        {
            _InventoryMenu.UpdateItemDescription(_SlotData.ItemData.Description);
            _InventoryMenu.UpdateItemName(_SlotData.ItemData.ItemName);
        }
    }

    private void ItemUnfocused()
    {
        if (_InventoryMenu != null)
        {
            _InventoryMenu.UpdateItemDescription("");
            _InventoryMenu.UpdateItemName("");
        }
    }

    private void ItemPressed()
    {
        if (_SlotData != null && _SlotData.ItemData != null)
        {
            Character user = _SlotData.User;
            if (user == null)
            {
                Logger.Error("InventorySlotUI: No user assigned to slot.");
                return;
            }

            bool wasUsed = _SlotData.ItemData.Use(user);

            if (wasUsed == false)
            {
                return;
            }
            _SlotData.Quantity -= 1;

            if (_SlotData.Quantity <= 0)
            {
                _SlotData.ItemData = null;
                _SlotData.Quantity = 0;
            }

            SetSlotData(_SlotData);
        }
    }
}
