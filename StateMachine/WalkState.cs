using Godot;

public partial class WalkState : CharacterState
{
    [Export]
    public BaseMovementBehavior _MovementBehavior; //Movement Behavior of the Character, for possible individual behave on individual Characters

    private IMovementComponent _MovementComponent;

    public override void _Ready()
    {
        base._Ready();
        _MovementComponent = Owner.FindChild("MovementComponent", recursive: true) as IMovementComponent;
        if (_MovementComponent == null)
        {
            GD.PrintErr("WalkState: No IMovementComponent found on " + Owner.Name);
        }
    }

    public override void Enter()
    {
        //GD.Print("Walk State Entered with" + _MovementBehavior.Name);
        base.Enter();
        if (_StateMachine._AnimationPlayer != null)
        {
            if (_StateMachine._AnimationPlayer.HasAnimation("Idle"))
            {
                _StateMachine._AnimationPlayer.Play("Idle");
            }
            else
            {
                _StateMachine._AnimationPlayer.Play("idle_down");
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Owner.GetType() != typeof(Player))
        {
            _StateMachine._CurrentDirection = _MovementBehavior.GetNextDirection();
        }

        _MovementComponent?.HandleMovement(_StateMachine._CurrentDirection);
        StartAnimation("move_");
    }
}