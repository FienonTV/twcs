using Godot;

public abstract partial class BaseMovementBehavior : Node, IMovementBehavior
{
    [Export]
    protected Character OwnerCharacter;

    protected Player _CurrentScenePlayer;

    [Signal]
    public delegate void OnDirectionGotEventHandler(Vector2 direction);

    public override void _Ready()
    {
        var players = GetTree()?.GetNodesInGroup("Player");
        if (players != null && players.Count > 0)
        {
            _CurrentScenePlayer = players[0] as Player;
            if (_CurrentScenePlayer != null)
            {
                Logger.Debug("BaseMovementBehavior: Player found.");
            }
        }
        else
        {
            Logger.Warning("BaseMovementBehavior: No player in group 'Player'. This is normal in editor or test scenes without a spawned player.");
        }
    }

    public virtual Vector2 GetNextDirection() { return default(Vector2); }

    public void CallOnDirectionGot(Vector2 direction)
    {
        Logger.Debug("BaseMovementBehavior: Emitting OnDirectionGot.");
        EmitSignal(SignalName.OnDirectionGot, direction);
    }
}
