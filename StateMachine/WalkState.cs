using Godot;

public partial class WalkState : CharacterState
{
    [Export]
    public BaseMovementBehavior _MovementBehavior; //Movement Behavior of the Character, for possible individual behave on individual Characters

    //private Vector2 _TargetDirection;

    public override void Enter()
    {
        //GD.Print("Walk State Entered with" + _MovementBehavior.Name);
        base.Enter();
        if (_StateMachine._AnimationPlayer.HasAnimation("Idle"))
        {
            _StateMachine._AnimationPlayer.Play("Idle");
        }
        else
        {
            _StateMachine._AnimationPlayer.Play("idle_down");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Owner.GetType() != typeof(Player))
        {
            _StateMachine._CurrentDirection = _MovementBehavior.GetNextDirection();
        }
        PlayerMovementComponent cmc = Owner.GetNode<PlayerMovementComponent>("MovementComponent");
        cmc.HandleMovement(_StateMachine._CurrentDirection);
        StartAnimation("move_");
    }
}