using Godot;
public partial class WorldInitialization : Node2D
{
    Player _PlayerInWorld; 

    public override void _Ready()
    {
           _PlayerInWorld = GameManager._PlayerScene.Instantiate() as Player;
           AddChild(_PlayerInWorld);
           _PlayerInWorld.GlobalPosition = new Vector2(150,150);
    }

}
