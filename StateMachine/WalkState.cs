using Godot;

public partial class WalkState : CharacterState
{
    [Export]
    public BaseMovementBehavior MovementBehavior;

    private IMovementComponent _MovementComponent;

    public override void _Ready()
    {
        base._Ready();
        if (Owner == null)
        {
            Logger.Error("WalkState: Owner is null. State will not function.");
            return;
        }
        if (_MovementComponent == null)
        {
            _MovementComponent = Owner.FindChild("MovementComponent", recursive: true) as IMovementComponent;
            if (_MovementComponent == null)
            {
                Logger.Error($"WalkState: No IMovementComponent found on '{Owner.Name}'.");
            }
        }
    }

    public override void Enter()
    {
        base.Enter();
        if (StateMachine.AnimationPlayer != null)
        {
            if (StateMachine.AnimationPlayer.HasAnimation("Idle"))
            {
                StateMachine.AnimationPlayer.Play("Idle");
            }
            else
            {
                StateMachine.AnimationPlayer.Play("idle_down");
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_MovementComponent == null)
        {
            return;
        }

        if (MovementBehavior != null)
        {
            StateMachine.CurrentDirection = MovementBehavior.GetNextDirection();
        }

        _MovementComponent.HandleMovement(StateMachine.CurrentDirection);
        StartAnimation("move_");
    }
}
