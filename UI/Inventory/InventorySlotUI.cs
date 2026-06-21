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
    TextureRect _TextureRect;
    Label _QuantityLabel;

    inventory_menu _InventoryMenu;

    public override void _Ready()
    {
        _TextureRect = GetNodeOrNull<TextureRect>("TextureRect");
        _QuantityLabel = GetNodeOrNull<Label>("QuantityLabel");
        _InventoryMenu = GetNodeOrNull<inventory_menu>("/root/InventoryMenu");

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

        if (_SlotData == null)
        {
            return;
        }

        if (_TextureRect != null)
        {
            _TextureRect.Texture = _SlotData._ItemData._Texture;
        }
        if (_QuantityLabel != null)
        {
            _QuantityLabel.Text = _SlotData._Quantity.ToString();
        }
    }

    private void ItemFocused()
    {
        if (_SlotData != null && _SlotData._ItemData != null && _InventoryMenu != null)
        {
            _InventoryMenu.updateItemDescription(_SlotData._ItemData._Description);
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
        if (_SlotData != null && _SlotData._ItemData != null)
        {
            Player user = GameManager.getPlayer();
            if (user == null)
            {
                GD.PrintErr("InventorySlotUI: No player found to use item.");
                return;
            }

            bool wasUsed = _SlotData._ItemData.Use(user);

            if (wasUsed == false)
            {
                return;
            }
            _SlotData._Quantity -= 1;

            if (_SlotData._Quantity <= 0)
            {
                _SlotData._ItemData = null;
                _SlotData._Quantity = 0;
                if (_TextureRect != null)
                {
                    _TextureRect.Texture = null;
                }
            }

            if (_QuantityLabel != null)
            {
                _QuantityLabel.Text = _SlotData._Quantity > 0 ? _SlotData._Quantity.ToString() : "";
            }

        }
    }


}
