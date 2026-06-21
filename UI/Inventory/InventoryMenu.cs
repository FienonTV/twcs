using Godot;
using System;

public partial class InventoryMenu : CanvasLayer
{
    public bool IsOpen = false;

    [Signal]
    public delegate void InventoryActiveEventHandler();

    [Signal]
    public delegate void InventoryHiddenEventHandler();

    [Export]
    private InventoryUI _InventoryUI;

    [Export]
    private Label ItemDescriptionLabel;

    [Export]
    private Label ItemNameLabel;

    private InputHandler _InputHandler;

    public Character CurrentUser { get; set; }

    public override void _Ready()
    {
        _InputHandler = Services.Get<InputHandler>();
        if (_InputHandler != null)
        {
            _InputHandler._ToggleInventoryRequested += ToggleInventory;
        }
        else
        {
            Logger.Error("InventoryMenu: InputHandler autoload not found.");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel") && IsOpen)
        {
            HideInventory();
        }
    }

    public void ToggleInventory()
    {
        if (IsOpen)
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
            Logger.Warning("InventoryMenu: No CurrentUser set; cannot open inventory.");
            return;
        }

        this.Show();
        IsOpen = true;
        _InventoryUI?.UpdateInventory();
        EmitSignal(SignalName.InventoryActive);

        if (ItemDescriptionLabel != null)
        {
            ItemDescriptionLabel.Text = "";
        }
        if (ItemNameLabel != null)
        {
            ItemNameLabel.Text = "";
        }
    }

    public void HideInventory()
    {
        this.Hide();
        IsOpen = false;
        EmitSignal(SignalName.InventoryHidden);
    }

    public void updateItemDescription(string description)
    {
        if (ItemDescriptionLabel != null)
        {
            ItemDescriptionLabel.Text = description;
        }
    }

    public void updateItemName(string name)
    {
        if (ItemNameLabel != null)
        {
            ItemNameLabel.Text = name;
        }
    }
}
