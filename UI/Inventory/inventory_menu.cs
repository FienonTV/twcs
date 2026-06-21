using Godot;
using System;


public partial class inventory_menu : CanvasLayer
{
    public bool _IsOpen = false;

    [Signal]
    public delegate void InventoryActiveEventHandler();

    [Signal]
    public delegate void InventoryHiddenEventHandler();

    public Character CurrentUser { get; set; }

    public override void _Ready()
    {
        Visible = false;
        ProcessMode = ProcessModeEnum.Always;
        var inputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");
        if (inputHandler != null)
        {
            inputHandler._ToggleInventoryRequested += ToggleInventory;
        }
        else
        {
            Logger.Error("inventory_menu: InputHandler autoload not found.");
        }
    }

    public void ToggleInventory()
    {
        if (_IsOpen)
        {
            HideInventory();
        }
        else
        {
            ShowInventory();
        }
    }

    public void ShowInventory()
    {
        if (CurrentUser == null)
        {
            Logger.Warning("inventory_menu: No CurrentUser set; cannot open inventory.");
            return;
        }

        _IsOpen = true;
        Visible = true;
        GetTree().Paused = true;
        EmitSignal(SignalName.InventoryActive);
    }

    public void HideInventory()
    {
        _IsOpen = false;
        Visible = false;
        GetTree().Paused = false;
        EmitSignal(SignalName.InventoryHidden);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("inventory") && _IsOpen)
        {
            HideInventory();
            GetViewport().SetInputAsHandled();
        }
    }

    public void updateItemDescription(string description)
    {
        Label itemDescriptionLabel = GetNodeOrNull<Label>("ItemDescription");
        if (itemDescriptionLabel != null)
        {
            itemDescriptionLabel.Text = description;
        }
    }
}
