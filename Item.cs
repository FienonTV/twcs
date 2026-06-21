/* This Class defines the Item spawned in the World. With Collision and Pickup */
using Godot;


public partial class Item : CharacterBody2D
{

	[Export]
	public ItemDataResource _ItemData;
	CollectableComponent _CollectableComponent;
	Sprite2D _Sprite2D;

	bool isMoving = true;

	public override void _Ready()
	{

		base._Ready();
		_CollectableComponent = FindChild("CollectableComponent", true) as CollectableComponent;
		_Sprite2D = FindChild("Sprite2D", true) as Sprite2D;


		UpdateTexture();

		if (_CollectableComponent == null)
		{
			Logger.Error("Item: " + Name + "CollectableComponent == null");
		}
		if (_Sprite2D == null)
		{
			Logger.Error("Item: " + Name + "_Sprite2D == null");
		}

		if (Engine.IsEditorHint())
		{
			return;
		}

		_CollectableComponent.OnItemPickedUp += ItemPickedup;

	}

	public override void _PhysicsProcess(double delta)
	{
		var collision_info = MoveAndCollide(new Vector2((float)(Velocity.X * delta), (float)(Velocity.Y * delta)));
		if (collision_info != null)
		{
			Velocity = Velocity.Bounce(collision_info.GetNormal());
		}
		Velocity -= new Vector2((float)(Velocity.X * delta), (float)(Velocity.Y * delta)) * 4;
		isMoving = false;
	}


	private void ItemPickedup()
	{	
		if(!isMoving) {
		_CollectableComponent.BodyEntered -= _CollectableComponent.OnBodyEntered;
		_CollectableComponent.OnItemPickedUp -= ItemPickedup;
		Visible = false;
		//await _AduioStreamPlayer.finished() TODO: When adding sounds this has to be in here
		QueueFree();
		}
		
	}


	public void UpdateTexture()
	{
		if (_ItemData != null && _Sprite2D != null)
		{
			_Sprite2D.Texture = _ItemData._Texture;
		}

		if (_ItemData == null)
		{
			Logger.Error("Item: " + Name + "_ItemData == null in UpdateTexture()");
		}
		if (_Sprite2D == null)
		{
			Logger.Error("Item: " + Name + " _Sprite2D == null in UpdateTexture()");
		}
	}


}
