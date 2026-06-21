using System;
using Godot;

public partial class PlayerStateMachine : CharacterStateMachine
{
    InputHandler _InputHandler;

    public override void _Ready()
    {
        base._Ready();
        _InputHandler = GetNode("/root/InputHandler") as InputHandler;

        _InputHandler._OnMoveInput += OnInputHandlerMoveInput;
        _InputHandler._NoMovement += OnInputHandlerNoMovement;
    }

    private void OnInputHandlerNoMovement()
    {
        if (_CurrentState.GetType() != typeof(IdleState))
        {
            ChangeState("Idle");
        }
    }

    private void OnInputHandlerMoveInput(Vector2 direction)
    {
        _CurrentDirection = direction.Normalized();

        if (_CurrentState.GetType() != typeof(WalkState))
        {
            ChangeState("Walk");
        }
    }


}