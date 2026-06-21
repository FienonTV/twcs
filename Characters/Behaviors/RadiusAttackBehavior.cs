using Godot;

public partial class RadiusAttackBehavior : BaseAttackCondition
{

    [Export] EnemyDetectionArea _EnemyDetectionArea;
    public override bool ShouldAttack()
    {
        if (_EnemyDetectionArea.checkForEnemies().Count > 0)
        {
            return true;
        }
        else
        {
            return false;

        }
    }
}