using Godot;

public partial class IdleState : State
{
    public override void Enter(Vector2 lastMovementDirection, AnimationController animationController)
    {
        _IsCurrentState = true;
        _MovementDirection = lastMovementDirection;
        _AnimationController = animationController;
        StartAnimation();
    }

    public override void StartAnimation()
    {
        if (DirectionForAnimation.TryGetValue(_MovementDirection, out string animationName))
        {
            _AnimationController?.PlayAnimation("idle_" + animationName);
        }
    }

    public override void Exit()
    {
        _IsCurrentState = false;
        GetParent<StateMachine>().ReturnToIdle();
        _AnimationController.StopAnimation();
    }
}