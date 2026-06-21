using Godot;

public partial class HandItem : Node2D
{
	[Export]
	public data_types.HandItemsTypes _HandItemCategory;

	public HitBoxComponent _HitBoxComponent;
	public int _Damage = 1;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
