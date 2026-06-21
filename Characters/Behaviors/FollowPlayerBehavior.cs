using Godot;

public partial class FollowPlayerBehavior : BaseMovementBehavior
{
    private Node2D _Target;

    public override void _Ready()
    {
        base._Ready();
    }

    public override Vector2 GetNextDirection()
    {
        if (_CharacterParent == null)
        {
            GD.PrintErr("FollowPlayerBehavior: _CharacterParent is null.");
            return Vector2.Zero;
        }

        if (_CharacterParent.navigationAgent2D == null)
        {
            GD.PrintErr("FollowPlayerBehavior: navigationAgent2D is null on " + _CharacterParent.Name);
            return Vector2.Zero;
        }

        if (_Target == null)
        {
            _Target = GameManager.getPlayer();
            if (_Target == null)
            {
                return Vector2.Zero;
            }
        }

        if (!IsInstanceValid(_Target))
        {
            _Target = null;
            return Vector2.Zero;
        }

        _CharacterParent.navigationAgent2D.TargetPosition = _Target.GlobalPosition;
        Vector2 nextPathPoint = _CharacterParent.navigationAgent2D.GetNextPathPosition();
        Vector2 direction = _CharacterParent.GlobalPosition.DirectionTo(nextPathPoint);
        return direction;
    }
}
