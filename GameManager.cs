using Godot;

public partial class GameManager : Node
{

    public static PackedScene _PlayerScene;
    private static Player _Player;


    public override void _Ready()
    {
        _PlayerScene = GD.Load<PackedScene>("res://Characters/Player/Player.tscn");
    
        _Player = _PlayerScene.Instantiate() as Player;


    }

    public static Player getPlayer() {
        return _Player;
    }
}
