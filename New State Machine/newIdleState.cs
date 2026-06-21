
public partial class newIdleState : newState
{
    public override void Enter()
    {
        base.Enter();
        Owner.SetPhysicsProcess(true);
        if (_StateMachine._AnimationPlayer.HasAnimation("Idle"))
            _StateMachine._AnimationPlayer.Play("Idle");
        else
            _StateMachine._AnimationPlayer.Play("idle_down");
    }

    public override void Exit()
    {
        base.Exit();
        Owner.SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        StartAnimation("idle_");
    }
}