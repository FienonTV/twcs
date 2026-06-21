using Godot;

public abstract partial class BaseMovementBehavior : Node, IMovementBehavior
{
    [Export]
    protected Character _CharacterParent; //The Character that this Behavior is attached to

    protected Player _CurrentScenePlayer;

    [Signal]
    public delegate void OnDirectionGotEventHandler(Vector2 direction);

    public override void _Ready()
    {
        _CurrentScenePlayer = GetTree().GetNodesInGroup("Player")[0] as Player;
        if (_CurrentScenePlayer != null)
        {
            GD.Print("Player Found");
        }
    }

    public virtual Vector2 GetNextDirection() { return default(Vector2); }

    public void CallOnDirectionGot(Vector2 direction)
    {
        GD.Print("I wanna call on Direction Got");
        // _OnDirectionGot.Invoke(direction);
        EmitSignal(SignalName.OnDirectionGot, direction);
    }
}
