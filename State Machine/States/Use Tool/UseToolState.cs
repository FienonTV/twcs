using Godot;

public partial class UseToolState : State


{
    public override void Enter(Vector2 lastMovementDirection, AnimationController animationController)
    {
        if (!_IsCurrentState)
        {
            _IsCurrentState = true;


            _LastMovementeDirection = GetParent<StateMachine>()._CharacterParent._CurrentLookingDirection;
            _AnimationController = animationController;
            StartAnimation();
        }
    }

    public override void StartAnimation()
    {
        if (DirectionForAnimation.TryGetValue(_LastMovementeDirection, out string animationName))
        {
            if (!_AnimationController.IsAnimationPlaying())
            {


                _AnimationController?.PlayAnimation("useTool_" + animationName);
            }
        }
    }

    public override void Exit()
    {
        _IsCurrentState = false;


        GetParent<StateMachine>().ReturnToIdle();
        _AnimationController?.StopAnimation();
    }
}

