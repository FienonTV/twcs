using Godot;

public partial class GameManager : Node
{
    public static PackedScene _PlayerScene;
    private static Player _Player;

    public override void _Ready()
    {
        _PlayerScene = GD.Load<PackedScene>("res://Characters/Player/Player.tscn");
    }

    public static void RegisterPlayer(Player player)
    {
        _Player = player;
        if (player != null && EventBus.Instance != null)
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.PlayerSpawned, player);
        }
    }

    public static Player getPlayer()
    {
        if (_Player == null)
        {
            GD.PrintErr("GameManager: No Player registered.");
        }
        return _Player;
    }
}
