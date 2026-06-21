using System;
using Godot;

public partial class InputHandler : Node
{
    public event Action<Vector2> _OnMoveInput;
    public event Action _OnUseInput;
    public event Action _OnInteractionInput;
    public event Action _NoMovement;

    public event Action _ToggleInventoryRequested;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("inventory"))
        {
            _ToggleInventoryRequested?.Invoke();
            GetViewport().SetInputAsHandled();
        }
    }

    public override void _Process(double delta)
    {
        Vector2 movementInput = new Vector2(
            Input.GetActionStrength("MoveRight") - Input.GetActionStrength("MoveLeft"),
            Input.GetActionStrength("MoveDown") - Input.GetActionStrength("MoveUp")
        ).Normalized();

        if (movementInput != Vector2.Zero)
        {
            _OnMoveInput?.Invoke(movementInput);
        }

        if (Input.IsActionJustReleased("MoveRight") || Input.IsActionJustReleased("MoveLeft") || Input.IsActionJustReleased("MoveUp") || Input.IsActionJustReleased("MoveDown"))
        {
            _NoMovement?.Invoke();
        }

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
