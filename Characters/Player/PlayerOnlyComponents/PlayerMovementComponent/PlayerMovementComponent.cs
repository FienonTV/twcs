using System;
using Godot;

public partial class PlayerMovementComponent : Node, IMovementComponent
{
    private CharacterBody2D _CharacterBody;

    [Export] private int _MovingSpeed = 100;

    public override void _Ready()
    {
        _CharacterBody = GetParent() as CharacterBody2D;
        if (_CharacterBody == null)
        {
            Logger.Error("PlayerMovementComponent: Parent is not a CharacterBody2D. Movement disabled.");
        }
    }

    public void HandleMovement(Vector2 direction)
    {
        if (_CharacterBody == null)
        {
            Logger.Error("PlayerMovementComponent: No CharacterBody2D to move.");
            return;
        }

        if (direction != Vector2.Zero)
        {
            _CharacterBody.Velocity = direction * _MovingSpeed;
        }
        else
        {
            _CharacterBody.Velocity = Vector2.Zero;
        }

        _CharacterBody.MoveAndSlide();
    }
}
