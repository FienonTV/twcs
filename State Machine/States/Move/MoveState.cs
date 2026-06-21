using Godot;

public partial class MoveState : State
{
    // This method is called when the state is entered
    public override void Enter(Vector2 lastMovementDirection, AnimationController animationController)
    {
        _IsCurrentState = true; // Mark this state as the current state
        _MovementDirection = lastMovementDirection; // Store the last movement direction
        _AnimationController = animationController; // Store the reference to the animation controller
        StartAnimation(); // Start the movement animation
    }

    // This method starts the appropriate animation based on the movement direction
    public override void StartAnimation()
    {
        // Try to get the animation name based on the movement direction
        if (DirectionForAnimation.TryGetValue(_MovementDirection, out string animationName))
        {
            // Play the corresponding animation
            _AnimationController?.PlayAnimation("move_" + animationName);
            // Update the character's current looking direction
            _StateMachine._CharacterParent._CurrentLookingDirection = _MovementDirection;

            // Store the last animation direction
            lastAnimationDirection = _MovementDirection;
        }
        else
        {
            // If the direction is not found, play the last animation direction
            if (lastAnimationDirection == Vector2.Right)
            {
                _AnimationController?.PlayAnimation("move_right");
                _StateMachine._CharacterParent._CurrentLookingDirection = Vector2.Right;
            }
            else if (lastAnimationDirection == Vector2.Left)
            {
                _AnimationController?.PlayAnimation("move_left");
                _StateMachine._CharacterParent._CurrentLookingDirection = Vector2.Left;
            }
            else if (lastAnimationDirection == Vector2.Up)
            {
                _AnimationController?.PlayAnimation("move_up");
                _StateMachine._CharacterParent._CurrentLookingDirection = Vector2.Up;
            }
            else if (lastAnimationDirection == Vector2.Down)
            {
                _AnimationController?.PlayAnimation("move_down");
                _StateMachine._CharacterParent._CurrentLookingDirection = Vector2.Down;
            }
        }
    }

    // This method is called when the state is exited
    public override void Exit()
    {
        // Add any cleanup code here if needed
    }
}