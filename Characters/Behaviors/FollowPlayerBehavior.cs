using Godot;
public partial class FollowPlayerBehavior : BaseMovementBehavior
{
    Vector2 _TargetPosition = Vector2.Zero; //The target position for the Character to move to

    /**
    ** Returns the next direction Vector2 for the Character to move towards the Player
    **/

    public override void _Ready()
    {
        base._Ready();

    }
    public override Vector2 GetNextDirection()
    {

        _CharacterParent.navigationAgent2D.TargetPosition = _CurrentScenePlayer.GlobalPosition;

        // Get the next path point to the target from the NavigationAgent2D
        Vector2 nextPathPoint = _CharacterParent.navigationAgent2D.GetNextPathPosition();

        // Calculate the direction Vector2 to the next path point
        Vector2 direction = _CharacterParent.GlobalPosition.DirectionTo(nextPathPoint);
        return direction;
    }

}

