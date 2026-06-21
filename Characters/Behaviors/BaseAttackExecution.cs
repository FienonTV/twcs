using Godot;
using System;

public abstract partial class BaseAttackExecution : Node, IAttackExecution
{
    public event Action<Vector2> _OnMovementNeeded;
    public event Action _OnAttackPossible;
    public virtual void ExecuteAttack(Vector2 currentPosition, Vector2 targetPosition)
    {

    }

    protected void CallOnMovementNeeded(Vector2 direction)
    {
        _OnMovementNeeded?.Invoke(direction);
    }

    protected void CallOnAttackPossible()
    {
        _OnAttackPossible?.Invoke();
    }
}