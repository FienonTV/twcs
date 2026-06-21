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
        if (OwnerCharacter == null)
        {
            Logger.Error("OwnerCharacter is null");
            return Vector2.Zero;
        }
        if (OwnerCharacter.navigationAgent2D == null)
        {
            Logger.Error("OwnerCharacter.navigationAgent2D is null");
            return Vector2.Zero;
        }

        // Check if the Character is near enough to the target position

        if (OwnerCharacter.navigationAgent2D.IsNavigationFinished())
        {
            Logger.Debug("Calculating new Target Position");
            _TargetPosition = NavigationServer2D.MapGetRandomPoint(OwnerCharacter.navigationAgent2D.GetNavigationMap(), OwnerCharacter.navigationAgent2D.NavigationLayers, false);
        }

        // Set this target position as target for the NavigationAgent2D
        OwnerCharacter.navigationAgent2D.TargetPosition = _TargetPosition;

        // Get the next path point to the target from the NavigationAgent2D
        Vector2 nextPathPoint = OwnerCharacter.navigationAgent2D.GetNextPathPosition();

        // Calculate the direction Vector2 to the next path point
        Vector2 direction = OwnerCharacter.GlobalPosition.DirectionTo(nextPathPoint);

        /*
        * Nur Gerade laufen
         // Ensure the movement is only vertical or horizontal
       if (Math.Abs(OwnerCharacter.GlobalPosition.X - nextPathPoint.X) > 1.0f)
       {
           // Move horizontally
           direction.Y = 0;
       }
       else if (Math.Abs(OwnerCharacter.GlobalPosition.Y - nextPathPoint.Y) > 1.0f)
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

