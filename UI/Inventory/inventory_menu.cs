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

        InputHandler inputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");
        if (inputHandler != null)
        {
            inputHandler._ToggleInventoryRequested += OnToggleInventoryRequested;
        }
        else
        {
            GD.PrintErr("inventory_menu: InputHandler autoload not found.");
        }
    }

    private void OnToggleInventoryRequested()
    {
        if (_IsOpen)
        {
            hideInventory();
        }
        else
        {
            showInventory();
        }
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
        if (_ItemDescriptionLabel != null)
        {
            _ItemDescriptionLabel.Text = newText;
        }
    }

    public override void _ExitTree()
    {
        InputHandler inputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");
        if (inputHandler != null)
        {
            inputHandler._ToggleInventoryRequested -= OnToggleInventoryRequested;
        }
        base._ExitTree();
    }
}
