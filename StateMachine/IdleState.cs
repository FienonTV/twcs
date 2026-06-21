
public partial class IdleState : CharacterState
{
    public override void Enter()
    {
        base.Enter();
        if (_StateMachine._AnimationPlayer.HasAnimation("Idle"))
            _StateMachine._AnimationPlayer.Play("Idle");
        else
            _StateMachine._AnimationPlayer.Play("idle_down");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        StartAnimation("idle_");
    }
}