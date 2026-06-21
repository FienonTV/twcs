using Godot;

public partial class FollowPlayerBehavior : BaseMovementBehavior
{
    private Node2D _Target;
    private GameManager _GameManager;

    public override void _Ready()
    {
        base._Ready();
        _GameManager = GetNodeOrNull<GameManager>("/root/GameManager");
    }

    public override Vector2 GetNextDirection()
    {
        if (_CharacterParent == null)
        {
            Logger.Error("FollowPlayerBehavior: _CharacterParent is null.");
            return Vector2.Zero;
        }

        if (_CharacterParent.navigationAgent2D == null)
        {
            Logger.Error($"FollowPlayerBehavior: navigationAgent2D is null on '{_CharacterParent.Name}'.");
            return Vector2.Zero;
        }

        if (_Target == null)
        {
            _Target = _GameManager?.Player;
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
