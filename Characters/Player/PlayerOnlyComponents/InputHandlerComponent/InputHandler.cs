using System;
using Godot;

public partial class InputHandler : Node
{
    public event Action<Vector2> _OnMoveInput;
    public event Action _OnUseInput;

    public event Action _OnInteractionInput;

    public event Action _NoMovement;

    inventory_menu _InventoryMenu;

    public override void _Ready()
    {
        _InventoryMenu = GetNodeOrNull<inventory_menu>("/root/InventoryMenu");
        if (_InventoryMenu == null)
        {
            GD.PrintErr("InputHandler: InventoryMenu autoload not found.");
        }
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("inventory"))
        {
            if (_InventoryMenu == null)
            {
                GD.PrintErr("InputHandler: InventoryMenu is null, cannot open inventory.");
                return;
            }

            if (_InventoryMenu._IsOpen == false)
            {
                _InventoryMenu.showInventory();
            }
            else
            {
                _InventoryMenu.hideInventory();
            }
            GetViewport().SetInputAsHandled();

        }
    }
    public override void _Process(double delta)
    {
        //GD.Print("Checking for Inputs");
        Vector2 movementInput = new Vector2(
            Input.GetActionStrength("MoveRight") - Input.GetActionStrength("MoveLeft"),
            Input.GetActionStrength("MoveDown") - Input.GetActionStrength("MoveUp")
        ).Normalized();

        //If there is movement calculated by the movementInput above, 
        //movement event is triggered and the movement direction  stored in the movementInput variable
        //is passed with the signal
        if (movementInput != Vector2.Zero)
        {
            // GD.Print("MovementDetected: " + movementInput.ToString());
            _OnMoveInput?.Invoke(movementInput);
        }

        if (Input.IsActionJustReleased("MoveRight") || Input.IsActionJustReleased("MoveLeft") || Input.IsActionJustReleased("MoveUp") || Input.IsActionJustReleased("MoveDown"))
        {
            _NoMovement?.Invoke();
        }
        //If Attack / UseItem Input is pressed, trigger the Event. 
        if (Input.IsActionJustPressed("UseEquippedItem") && GetTree().Paused == false)
        {
            _OnUseInput?.Invoke();
        }

        if (Input.IsActionJustReleased("Interact"))
        {
            _OnInteractionInput?.Invoke();
        }
    }
}