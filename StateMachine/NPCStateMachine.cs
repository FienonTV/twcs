using Godot;

public partial class NPCStateMachine : CharacterStateMachine
{
    [Export]
    private Area2D _DetectionArea;

    private Player _CurrentScenePlayer;

    public override void _Ready()
    {
        base._Ready();
        InitializePlayerReference();
    }

    private void InitializePlayerReference()
    {
        if (Owner is Player player)
        {
            _CurrentScenePlayer = player;
            return;
        }

        Godot.Collections.Array<Node> players = GetTree().GetNodesInGroup("Player");
        if (players.Count > 0)
        {
            _CurrentScenePlayer = players[0] as Player;
        }

        if (_CurrentScenePlayer == null)
        {
            Logger.Error("NPCStateMachine: Could not find a Player in the scene.");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_CurrentScenePlayer == null)
        {
            return;
        }

        if ((Owner as Character)?.GlobalPosition.DistanceTo(_CurrentScenePlayer.GlobalPosition) < 300)
        {
            if (_CurrentState.GetType() != typeof(FollowState))
            {
                Logger.Debug("Changing State to Follow");
                ChangeState("Follow");
            }
        }
        else
        {
            if (_CurrentState.GetType() != typeof(WalkState))
            {
                Logger.Debug("Changing State to Walk");
                ChangeState("Walk");
            }
        }
    }
}
