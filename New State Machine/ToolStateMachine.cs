using Godot;

public partial class ToolStateMachine : newStateMachine
{
    InputHandler _InputHandler;

    public override void _Ready()
    {
        base._Ready();
        GD.Print("This tool is part of" + Owner.Owner.Name);
        _InputHandler = GetNode("/root/InputHandler") as InputHandler;

        _InputHandler._OnUseInput += OnUseInput;

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _CurrentDirection = Owner.Owner.GetNode<newStateMachine>("StateMachine")._CurrentDirection;
    }

    private async void OnUseInput()
    {
        if (_CurrentState.GetType() != typeof(UseToolState))
        {
            GD.Print("UseTool");
            ChangeState("UseTool");
            await ToSignal(_AnimationPlayer, "animation_finished");
            GD.Print("Idle");
            ChangeState("Idle");
        }
    }
}
