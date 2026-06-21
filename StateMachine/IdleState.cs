
public partial class IdleState : CharacterState
{
    public override void Enter()
    {
        base.Enter();
        if (StateMachine.ActiveAnimationPlayer.HasAnimation("Idle"))
            StateMachine.ActiveAnimationPlayer.Play("Idle");
        else
            StateMachine.ActiveAnimationPlayer.Play("idle_down");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        StartAnimation("idle_");
    }
}