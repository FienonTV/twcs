using Godot;
using System;

public partial class inventory_menu : CanvasLayer
{
    public bool _IsOpen = false;

    [Signal]
    public delegate void InventoryActiveEventHandler();

    [Signal]
    public delegate void InventoryHiddenEventHandler();

    Label _ItemDescriptionLabel;



    public override void _Ready()
    {
        _ItemDescriptionLabel = FindChild("ItemDescription", true) as Label;
        hideInventory();
    }

    public void showInventory()
    {
        GetTree().Paused = true;
        Visible = true;
        _IsOpen = true;
        EmitSignal(SignalName.InventoryActive);
    }

    public void hideInventory()
    {
        GetTree().Paused = false;
        Visible = false;
        _IsOpen = false;
        EmitSignal(SignalName.InventoryHidden);
    }

    public void updateItemDescription(String newText)
    {
        _ItemDescriptionLabel.Text = newText;
    }
}