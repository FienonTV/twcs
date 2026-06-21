using Godot;

public partial class EventBus : Node
{
    public static EventBus Instance { get; private set; }

    [Signal]
    public delegate void PlayerSpawnedEventHandler(Player player);

    [Signal]
    public delegate void PlayerHealthChangedEventHandler(Player player, int health);

    [Signal]
    public delegate void EntityDiedEventHandler(Node entity);

    public override void _Ready()
    {
        Instance = this;
    }
}
