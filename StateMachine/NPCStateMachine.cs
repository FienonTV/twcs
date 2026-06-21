using Godot;
public partial class NPCStateMachine : CharacterStateMachine
{
    public override void _Process(double delta)
    {
        if ((Owner as Character).GlobalPosition.DistanceTo(_CurrentScenePlayer.GlobalPosition) < 300)
        {
            if (_CurrentState.GetType() != typeof(FollowState))
            {
                GD.Print("Changing State to Follow");
                ChangeState("Follow");
            }
        }
        else
        {
            if (_CurrentState.GetType() != typeof(WalkState))
            {
                GD.Print("Changing State to Walk");
                ChangeState("Walk");
            }
        }
    }
}