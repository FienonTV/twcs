
public partial class IdleState : CharacterState
{
    public override void Enter()
    {
        base.Enter();
        if (StateMachine.AnimationPlayer.HasAnimation("Idle"))
            StateMachine.AnimationPlayer.Play("Idle");
        else
            StateMachine.AnimationPlayer.Play("idle_down");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        StartAnimation("idle_");
    }
}