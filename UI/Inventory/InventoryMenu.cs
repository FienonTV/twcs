using Godot;

public partial class InventoryMenu : CanvasLayer
{
    public bool IsOpen = false;

    [Export]
    private InventoryUI _InventoryUI;

    [Export]
    private Control _InventoryPanel;

    [Export]
    private Label _ItemDescriptionLabel;

    [Export]
    private Label _ItemNameLabel;

    private InputHandler _InputHandler;

    public Character CurrentUser { get; set; }
    public InventoryDataResource InventoryData { get; set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        HideInventory();

        if (_InventoryUI == null)
        {
            _InventoryUI = FindChild("GridContainer", true) as InventoryUI;
        }

        if (_InventoryPanel == null)
        {
            _InventoryPanel = GetNodeOrNull<Control>("Control");
        }

        if (_ItemDescriptionLabel == null)
        {
            _ItemDescriptionLabel = GetNodeOrNull<Label>("Control/ItemDescription");
        }

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
            GetViewport().SetInputAsHandled();
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

        Visible = true;
        if (_InventoryPanel != null)
        {
            _InventoryPanel.Visible = true;
        }

        IsOpen = true;
        _InventoryUI?.UpdateInventory();

        if (_ItemDescriptionLabel != null)
        {
            _ItemDescriptionLabel.Text = "";
        }
        if (_ItemNameLabel != null)
        {
            _ItemNameLabel.Text = "";
        }
    }

    public void HideInventory()
    {
        Visible = false;
        if (_InventoryPanel != null)
        {
            _InventoryPanel.Visible = false;
        }

        IsOpen = false;
        _InventoryUI?.ClearInventory();
    }

    public void UpdateItemDescription(string description)
    {
        if (_ItemDescriptionLabel != null)
        {
            _ItemDescriptionLabel.Text = description;
        }
    }

    public void UpdateItemName(string name)
    {
        if (_ItemNameLabel != null)
        {
            _ItemNameLabel.Text = name;
        }
    }
}
