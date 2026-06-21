using Godot;

public interface IAttackExecution
{
    void ExecuteAttack(Vector2 currentPosition, Vector2 targetPosition);
}