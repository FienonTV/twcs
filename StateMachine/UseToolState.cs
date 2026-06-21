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
            GD.Print("HitBoxComponent is null");
        }

        CallDeferred("SetCollisionShapeDisabled");
    }
    public override void Enter()
    {
        base.Enter();

        if (_StateMachine._AnimationPlayer.HasAnimation("Idle"))
            _StateMachine._AnimationPlayer.Play("Idle");
        else
            _StateMachine._AnimationPlayer.Play("idle_down");

        //SetHitComponentDirection();
        _HitBoxComponent._CollisionShape2D.Disabled = false;
    }

    public override void Exit()
    {
        base.Exit();
        _HitBoxComponent._CollisionShape2D.Disabled = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (_StateMachine._AnimationPlayer.CurrentAnimation.StartsWith("useTool_") == false)
        {
            //SetHitComponentDirection();
            StartAnimation("useTool_");
        }

    }

    public void SetCollisionShapeDisabled()
    {
        _HitBoxComponent._CollisionShape2D.Disabled = true;
    }


}