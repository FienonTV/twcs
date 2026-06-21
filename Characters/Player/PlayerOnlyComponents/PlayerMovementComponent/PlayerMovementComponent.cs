using System;
using Godot;

public partial class PlayerMovementComponent : Node, IMovementComponent
{
    //public event Action<string, Vector2> _OnMovingPerformed;

    public Player _Player;

    [Export] private int _MovingSpeed = 100;

    public override void _Ready()
    {
        _Player = GetParent() as Player;
        if (_Player == null)
        {
            GD.PrintErr("PlayerMovementComponent: Parent is not a Player.");
        }
    }

    public void HandleMovement(Vector2 direction)
    {
        if (_Player == null)
        {
            GD.PrintErr("PlayerMovementComponent: No Player to move.");
            return;
        }

        if (direction != Vector2.Zero)
        {
            _Player.Velocity = direction * _MovingSpeed;
        }
        else
        {
            _Player.Velocity = Vector2.Zero;
        }

        _Player.MoveAndSlide();
        //_OnMovingPerformed?.Invoke("MoveState", direction);
    }
}