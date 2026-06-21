using System;
using Godot;

public partial class PlayerStateMachine : CharacterStateMachine
{
    InputHandler _InputHandler;

    public override void _Ready()
    {
        base._Ready();
        _InputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");

        if (_InputHandler == null)
        {
            Logger.Error("PlayerStateMachine: InputHandler autoload not found.");
            return;
        }

        _InputHandler._OnMoveInput += OnInputHandlerMoveInput;
        _InputHandler._NoMovement += OnInputHandlerNoMovement;
    }

    private void OnInputHandlerNoMovement()
    {
        if (_CurrentState?.GetType() != typeof(IdleState))
        {
            ChangeState("Idle");
        }
    }

    private void OnInputHandlerMoveInput(Vector2 direction)
    {
        CurrentDirection = direction.Normalized();

        if (_CurrentState?.GetType() != typeof(WalkState))
        {
            ChangeState("Walk");
        }
    }

    public override void _ExitTree()
    {
        if (_InputHandler != null)
        {
            _InputHandler._OnMoveInput -= OnInputHandlerMoveInput;
            _InputHandler._NoMovement -= OnInputHandlerNoMovement;
        }
        base._ExitTree();
    }
}
