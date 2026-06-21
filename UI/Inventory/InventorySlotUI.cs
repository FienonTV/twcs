using System.Runtime.CompilerServices;
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
    private TextureRect TextureRect;

    [Export]
    private Label QuantityLabel;

    private InventoryMenu _InventoryMenu;

    public override void _Ready()
    {
        _InventoryMenu = Services.Get<InventoryMenu>();

        if (TextureRect != null)
        {
            TextureRect.Texture = null;
        }
        if (QuantityLabel != null)
        {
            QuantityLabel.Text = "";
        }
        FocusEntered += ItemFocused;
        FocusExited += ItemUnfocused;
        Pressed += ItemPressed;
    }

    private void SetSlotData(SlotDataResource slotData)
    {
        _SlotData = slotData;

        if (_SlotData == null)
        {
            return;
        }

        if (TextureRect != null)
        {
            TextureRect.Texture = _SlotData.ItemData.Texture;
        }
        if (QuantityLabel != null)
        {
            QuantityLabel.Text = _SlotData.Quantity.ToString();
        }
    }

    private void ItemFocused()
    {
        if (_SlotData != null && _SlotData.ItemData != null && _InventoryMenu != null)
        {
            _InventoryMenu.updateItemDescription(_SlotData.ItemData.Description);
        }
    }

    private void ItemUnfocused()
    {
        if (_InventoryMenu != null)
        {
            _InventoryMenu.updateItemDescription("");
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
                if (TextureRect != null)
                {
                    TextureRect.Texture = null;
                }
            }

            if (QuantityLabel != null)
            {
                QuantityLabel.Text = _SlotData.Quantity > 0 ? _SlotData.Quantity.ToString() : "";
            }
        }
    }
}
