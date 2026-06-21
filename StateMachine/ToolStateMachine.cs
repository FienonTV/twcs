using Godot;

public partial class ToolStateMachine : CharacterStateMachine
{
    [Export]
    private CharacterStateMachine _OwnerStateMachine;

    InputHandler _InputHandler;

    public override void _Ready()
    {
        base._Ready();
        GD.Print("This tool is part of " + Owner?.Name);
        _InputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");
        if (_InputHandler != null)
        {
            _InputHandler._OnUseInput += OnUseInput;
        }
        else
        {
            GD.PrintErr("ToolStateMachine: InputHandler autoload not found.");
        }

        if (_AnimationPlayer != null)
        {
            _AnimationPlayer.AnimationFinished += OnAnimationFinished;
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
            GD.Print("UseTool");
            ChangeState("UseTool");
        }
    }

    private void OnAnimationFinished(StringName animationName)
    {
        if (_CurrentState?.GetType() == typeof(UseToolState))
        {
            GD.Print("Idle");
            ChangeState("Idle");
        }
    }

    public override void _ExitTree()
    {
        if (_InputHandler != null)
        {
            _InputHandler._OnUseInput -= OnUseInput;
        }

        if (_AnimationPlayer != null)
        {
            _AnimationPlayer.AnimationFinished -= OnAnimationFinished;
        }
        base._ExitTree();
    }
}
