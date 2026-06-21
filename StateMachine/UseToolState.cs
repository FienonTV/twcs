using Godot;

public partial class UseToolState : CharacterState
{
    [Export]
    private HitBoxComponent HitBoxComponent;

    public override void _Ready()
    {
        base._Ready();
        if (Owner == null)
        {
            Logger.Error("UseToolState: Owner is null. State will not function.");
            return;
        }

        if (HitBoxComponent == null)
        {
            HitBoxComponent = Owner.GetNodeOrNull<HitBoxComponent>("HitBoxComponent");
        }

        if (HitBoxComponent == null)
        {
            Logger.Error($"UseToolState: No HitBoxComponent found on '{Owner.Name}'.");
        }
        else
        {
            HitBoxComponent.DeactivateHitBox();
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

        HitBoxComponent?.ActivateHitBox();
    }

    public override void Exit()
    {
        base.Exit();
        HitBoxComponent?.DeactivateHitBox();
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
