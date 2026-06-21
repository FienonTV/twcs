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
            GD.PrintErr("UseToolState: No HitBoxComponent found on " + Owner.Name);
        }
        else
        {
            _HitBoxComponent.DeactivateHitBox();
        }
    }
    public override void Enter()
    {
        base.Enter();

        if (_StateMachine._AnimationPlayer != null)
        {
            if (_StateMachine._AnimationPlayer.HasAnimation("Idle"))
                _StateMachine._AnimationPlayer.Play("Idle");
            else
                _StateMachine._AnimationPlayer.Play("idle_down");
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
        if (_StateMachine._AnimationPlayer != null && _StateMachine._AnimationPlayer.CurrentAnimation.StartsWith("useTool_") == false)
        {
            StartAnimation("useTool_");
        }
    }


}
