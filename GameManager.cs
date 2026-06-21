using Godot;

public partial class GameManager : Node
{
    public PackedScene PlayerScene { get; private set; }

    private Player _player;

    public Player Player
    {
        get
        {
            if (_player == null)
            {
                Logger.Error("GameManager: No Player registered.");
            }
            return _player;
        }
    }

    public bool HasPlayer
    {
        get { return _player != null && IsInstanceValid(_player); }
    }

    public override void _Ready()
    {
        PlayerScene = GD.Load<PackedScene>(ResourcePaths.PlayerScene);
        Logger.Debug("GameManager ready.");
    }

    public void RegisterPlayer(Player player)
    {
        _player = player;
        if (player != null && EventBus.Instance != null)
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.PlayerSpawned, player);
        }
        Logger.Debug($"GameManager: Registered player '{player?.Name}'.");
    }
}
