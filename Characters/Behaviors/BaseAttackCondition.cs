using Godot;

public abstract partial class BaseAttackCondition : Node, IAttackCondition
{
    [Export]
    protected Character OwnerCharacter; //The Character that this Behavior is attached to
    public virtual bool ShouldAttack()
    {
        return false;
    }
}