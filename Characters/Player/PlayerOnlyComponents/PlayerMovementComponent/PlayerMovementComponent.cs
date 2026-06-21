using System;
using Godot;

public partial class PlayerMovementComponent : Node
{
    //public event Action<string, Vector2> _OnMovingPerformed;

    public Player _Player;

    [Export] private int _MovingSpeed = 100;

    public override void _Ready()
    {
        _Player = GetParent() as Player;
    }

    public void HandleMovement(Vector2 direction)
    {
        //Already checked before sending Singal in InputHandler, but double check if the Method is called somewhere else
        if (direction != Vector2.Zero)
        {
            _Player.Velocity = direction * _MovingSpeed;
        }

        else
        {
            _Player.Velocity = Vector2.Zero;
        }

        _Player.MoveAndSlide();
        //_OnMovingPerformed.Invoke("MoveState", direction);
    }
}