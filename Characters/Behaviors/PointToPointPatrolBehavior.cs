using System;
using Godot;
public partial class PointToPointPatrolBehavior : BaseMovementBehavior
{
    Vector2 _TargetPosition = Vector2.Zero; //The target position for the Character to move to

    /**
    ** Returns the next direction Vector2 for the Character to move to
    ** Sets a random point in the NavigationMap as target for the NavigationAgent2D
    ** Calculates the next path point to the target from the NavigationAgent2D
    * TODO: Check if this method observes obstacles
    **/
    public override Vector2 GetNextDirection()
    {
        if (_CharacterParent == null)
        {
            GD.PrintErr("_CharacterParent is null");
            return Vector2.Zero;
        }
        if (_CharacterParent.navigationAgent2D == null)
        {
            GD.PrintErr("_CharacterParent.navigationAgent2D is null");
            return Vector2.Zero;
        }

        // Check if the Character is near enough to the target position

        if (_CharacterParent.navigationAgent2D.IsNavigationFinished())
        {
            GD.Print("Calculating new Target Position");
            _TargetPosition = NavigationServer2D.MapGetRandomPoint(_CharacterParent.navigationAgent2D.GetNavigationMap(), _CharacterParent.navigationAgent2D.NavigationLayers, false);
        }

        // Set this target position as target for the NavigationAgent2D
        _CharacterParent.navigationAgent2D.TargetPosition = _TargetPosition;

        // Get the next path point to the target from the NavigationAgent2D
        Vector2 nextPathPoint = _CharacterParent.navigationAgent2D.GetNextPathPosition();

        // Calculate the direction Vector2 to the next path point
        Vector2 direction = _CharacterParent.GlobalPosition.DirectionTo(nextPathPoint);

        /*
        * Nur Gerade laufen
         // Ensure the movement is only vertical or horizontal
       if (Math.Abs(_CharacterParent.GlobalPosition.X - nextPathPoint.X) > 1.0f)
       {
           // Move horizontally
           direction.Y = 0;
       }
       else if (Math.Abs(_CharacterParent.GlobalPosition.Y - nextPathPoint.Y) > 1.0f)
       {
           // Move vertically
           direction.X = 0;
       }
       else
       {
           // If close enough to the next path point, stop moving
           direction = Vector2.Zero;
       }*/

        // Normalize the direction to ensure consistent speed
        if (direction != Vector2.Zero)
        {
            direction = direction.Normalized();
        }

        return direction;
    }

}

