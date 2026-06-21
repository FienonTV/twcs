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
        _TextureRect = GetNode<TextureRect>("TextureRect");
        _QuantityLabel = GetNode<Label>("QuantityLabel");
        _InventoryMenu = GetNode<inventory_menu>("/root/InventoryMenu");

        _TextureRect.Texture = null;
        _QuantityLabel.Text = "";
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

        _TextureRect.Texture = _SlotData._ItemData._Texture;
        _QuantityLabel.Text = _SlotData._Quantity.ToString();
    }

    private void ItemFocused()
    {
        if (_SlotData != null && _SlotData._ItemData != null)
        {
            _InventoryMenu.updateItemDescription(_SlotData._ItemData._Description);
        }

    }

    private void ItemUnfocused()
    {
        _InventoryMenu.updateItemDescription("");
    }

    private void ItemPressed()
    {
        if (_SlotData != null && _SlotData._ItemData != null)
        {
            bool wasUsed = _SlotData._ItemData.Use();

            if (wasUsed == false)
            {
                return;
            }
            _SlotData._Quantity -= 1;
            _QuantityLabel.Text = _SlotData._Quantity.ToString();

        }
    }


}
