using Godot;

public partial class ToolStateMachine : CharacterStateMachine
{
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
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        CharacterStateMachine ownerStateMachine = GetOwnerStateMachine();
        if (ownerStateMachine != null)
        {
            _CurrentDirection = ownerStateMachine._CurrentDirection;
        }
    }

    private CharacterStateMachine GetOwnerStateMachine()
    {
        Node owner = Owner;
        if (owner == null)
        {
            return null;
        }

        CharacterStateMachine directMachine = owner.GetNodeOrNull<CharacterStateMachine>("StateMachine");
        if (directMachine != null)
        {
            return directMachine;
        }

        if (owner.Owner != null)
        {
            return owner.Owner.GetNodeOrNull<CharacterStateMachine>("StateMachine");
        }

        return null;
    }

    private async void OnUseInput()
    {
        if (_CurrentState?.GetType() != typeof(UseToolState))
        {
            GD.Print("UseTool");
            ChangeState("UseTool");
            await ToSignal(_AnimationPlayer, "animation_finished");
            GD.Print("Idle");
            ChangeState("Idle");
        }
    }
}
