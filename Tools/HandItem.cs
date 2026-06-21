using Godot;

public partial class HandItem : Node2D
{
    [Export]
    public DataTypes.HandItemsTypes HandItemCategory;

    [Export]
    public HitBoxComponent HitBoxComponent;

    [Export]
    public int Damage = 1;

    public override void _Process(double delta)
    {
    }
}
