using Godot;

public partial class WorldInitialization : Node2D
{
    [Export]
    public Vector2 PlayerSpawnPosition = new Vector2(150, 150);

    [Export]
    public NodePath PlayerParentPath = "../Y Sorted";

    private Player _PlayerInWorld;
    private GameManager _GameManager;

    public override void _Ready()
    {
        _GameManager = Services.Get<GameManager>();
        if (_GameManager == null || _GameManager.PlayerScene == null)
        {
            Logger.Error("WorldInitialization: GameManager not found or PlayerScene not loaded.");
            return;
        }

        _PlayerInWorld = _GameManager.PlayerScene.Instantiate() as Player;
        if (_PlayerInWorld == null)
        {
            Logger.Error("WorldInitialization: Failed to instantiate Player from GameManager.PlayerScene.");
            return;
        }

        Node playerParent = GetNodeOrNull(PlayerParentPath);
        if (playerParent == null)
        {
            Logger.Error($"WorldInitialization: Player parent node not found at {PlayerParentPath}. Spawning under self.");
            AddChild(_PlayerInWorld);
        }
        else
        {
            playerParent.AddChild(_PlayerInWorld);
        }

        _PlayerInWorld.GlobalPosition = PlayerSpawnPosition;
        _PlayerInWorld.ZIndex = 1;

        _GameManager.RegisterPlayer(_PlayerInWorld);

        Logger.Info($"WorldInitialization: Player spawned at {PlayerSpawnPosition}.");
    }
}
