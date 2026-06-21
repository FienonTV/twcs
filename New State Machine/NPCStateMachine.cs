using Godot;
public partial class NPCStateMachine : newStateMachine
{
    public override void _Process(double delta)
    {
        if ((Owner as Character).GlobalPosition.DistanceTo(_CurrentScenePlayer.GlobalPosition) < 300)
        {
            if (_CurrentState.GetType() != typeof(newFollowState))
            {
                GD.Print("Changing State to Follow");
                ChangeState("Follow");
            }
        }
        else
        {
            if (_CurrentState.GetType() != typeof(newWalkState))
            {
                GD.Print("Changing State to Walk");
                ChangeState("Walk");
            }
        }
    }
}