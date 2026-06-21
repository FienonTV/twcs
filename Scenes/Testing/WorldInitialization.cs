using Godot;

public partial class WorldInitialization : Node2D
{
    [Export]
    public Vector2 _PlayerSpawnPosition = new Vector2(150, 150);

    [Export]
    public NodePath _PlayerParentPath = "../Y Sorted";

    private Player _PlayerInWorld;

    public override void _Ready()
    {
        if (GameManager._PlayerScene == null)
        {
            GD.PrintErr("WorldInitialization: GameManager._PlayerScene is null. Cannot spawn player.");
            return;
        }

        _PlayerInWorld = GameManager._PlayerScene.Instantiate() as Player;
        if (_PlayerInWorld == null)
        {
            GD.PrintErr("WorldInitialization: Failed to instantiate Player from GameManager._PlayerScene.");
            return;
        }

        Node playerParent = GetNodeOrNull(_PlayerParentPath);
        if (playerParent == null)
        {
            GD.PrintErr("WorldInitialization: Player parent node not found at " + _PlayerParentPath + ". Spawning under self.");
            AddChild(_PlayerInWorld);
        }
        else
        {
            playerParent.AddChild(_PlayerInWorld);
        }

        _PlayerInWorld.GlobalPosition = _PlayerSpawnPosition;
        _PlayerInWorld.ZIndex = 1;

        GameManager.RegisterPlayer(_PlayerInWorld);

        GD.Print("WorldInitialization: Player spawned at " + _PlayerSpawnPosition);
    }
}
