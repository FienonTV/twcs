using Godot;

public partial class SimpleAttackExecution : BaseAttackExecution
{
    private float _StrikeDistance;
    public override void ExecuteAttack(Vector2 currentPosition, Vector2 targetPosition)
    {
        if (currentPosition.DistanceTo(targetPosition) > _StrikeDistance)
        {
            CallOnMovementNeeded((targetPosition - currentPosition).Normalized());
        }

        else
        {
            CallOnAttackPossible();
        }
    }
}