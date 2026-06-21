using Godot;
using System.Collections.Generic;



public abstract partial class State : Node
{

    public StateMachine _StateMachine { get; set; }
    protected Vector2 _MovementDirection { get; set; }

    public Vector2 lastAnimationDirection;

    protected Vector2 _LastMovementeDirection { get; set; }
    protected AnimationController _AnimationController;
    public virtual void Enter(Vector2 lastMovementDirection, AnimationController animationController) { }
    public virtual void Exit() { }
    public override void _PhysicsProcess(double delta) { }

    public bool _IsCurrentState = false;
    public void SetMovementDirection(Vector2 direction)
    {
        _MovementDirection = direction;
    }

    public void SetLastMovementDirection(Vector2 direction)
    {
        _LastMovementeDirection = direction;
    }

    protected Dictionary<Vector2, string> DirectionForAnimation = new Dictionary<Vector2, string>
    {
        { new Vector2(1,0), "right" },
        { new Vector2(-1,0), "left" },
        { new Vector2(0,1), "down" },
        { new Vector2(0,-1), "up" }
    };

    public virtual void StartAnimation()
    {

    }
}