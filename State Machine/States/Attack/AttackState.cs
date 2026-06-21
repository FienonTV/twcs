using Godot;

public partial class AttackState : State
{
    public override void Enter(
        Vector2 lastMovementDirection,
        AnimationController animationController
    )
    {
        //GD.Print("Entering Idle State");
    }

    public override void StartAnimation()
    {
        GD.Print("Möchte jetzt Attack Starten");
        if (DirectionForAnimation.TryGetValue(_MovementDirection, out string animationName))
        {
            GD.Print("Idle: I will Print following Animation now: " + "attack_" + animationName);
            _AnimationController?.PlayAnimation("attack_" + animationName);
        }
    }
}
