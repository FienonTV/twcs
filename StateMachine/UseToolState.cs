using Godot;

public partial class UseToolState : CharacterState
{

    private HitBoxComponent _HitBoxComponent;

    public override void _Ready()
    {
        base._Ready();
        _HitBoxComponent = Owner.GetNodeOrNull<HitBoxComponent>("HitBoxComponent");
        if (_HitBoxComponent == null)
        {
            Logger.Error("UseToolState: No HitBoxComponent found on " + Owner.Name);
        }
        else
        {
            _HitBoxComponent.DeactivateHitBox();
        }
    }
    public override void Enter()
    {
        base.Enter();

        if (StateMachine.AnimationPlayer != null)
        {
            if (StateMachine.AnimationPlayer.HasAnimation("Idle"))
                StateMachine.AnimationPlayer.Play("Idle");
            else
                StateMachine.AnimationPlayer.Play("idle_down");
        }

        _HitBoxComponent?.ActivateHitBox();
    }

    public override void Exit()
    {
        base.Exit();
        _HitBoxComponent?.DeactivateHitBox();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (StateMachine.AnimationPlayer != null && StateMachine.AnimationPlayer.CurrentAnimation.StartsWith("useTool_") == false)
        {
            StartAnimation("useTool_");
        }
    }


}
