using Godot;

public partial class ToolStateMachine : CharacterStateMachine
{
    [Export]
    private CharacterStateMachine _OwnerStateMachine;

    InputHandler _InputHandler;

    public override void _Ready()
    {
        base._Ready();
        Logger.Debug("This tool is part of " + Owner?.Name);
        _InputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");
        if (_InputHandler != null)
        {
            _InputHandler._OnUseInput += OnUseInput;
        }
        else
        {
            Logger.Error("ToolStateMachine: InputHandler autoload not found.");
        }

        if (AnimationPlayer != null)
        {
            AnimationPlayer.AnimationFinished += OnAnimationFinished;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_OwnerStateMachine != null)
        {
            _CurrentDirection = _OwnerStateMachine._CurrentDirection;
        }
    }

    private void OnUseInput()
    {
        if (_CurrentState?.GetType() != typeof(UseToolState))
        {
            Logger.Debug("UseTool");
            ChangeState("UseTool");
        }
    }

    private void OnAnimationFinished(StringName animationName)
    {
        if (_CurrentState?.GetType() == typeof(UseToolState))
        {
            Logger.Debug("Idle");
            ChangeState("Idle");
        }
    }

    public override void _ExitTree()
    {
        if (_InputHandler != null)
        {
            _InputHandler._OnUseInput -= OnUseInput;
        }

        if (AnimationPlayer != null)
        {
            AnimationPlayer.AnimationFinished -= OnAnimationFinished;
        }
        base._ExitTree();
    }
}
