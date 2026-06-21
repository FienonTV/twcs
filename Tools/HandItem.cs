using Godot;

public partial class HandItem : Node2D
{
	[Export]
	public DataTypes.HandItemsTypes HandItemCategory;

	public HitBoxComponent HitBoxComponent;
	public int Damage = 1;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
