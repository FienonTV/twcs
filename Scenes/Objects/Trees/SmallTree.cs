using Godot;

public partial class SmallTree : Sprite2D
{
	HurtBoxComponent _HurtBoxComponent;
	HealthComponent _HealthComponent;

	PackedScene _LogScene = ResourceLoader.Load<PackedScene>("res://Scenes/Objects/Trees/log.tscn");

	public override void _Ready()
	{
		_HurtBoxComponent = FindChild("HurtboxComponent", recursive: true) as HurtBoxComponent;
		_HealthComponent = FindChild("HealthComponent", recursive: true) as HealthComponent;

		if (_HurtBoxComponent != null)
		{
			_HurtBoxComponent._OnDamageRecived += ReceiveDamage;
		}

		if (_HealthComponent != null)
		{
			_HealthComponent._HealthEmpty += ZeroHealthReached;
		}
	}


	public async void ReceiveDamage(int damage)
	{
		_HealthComponent?.ChangeHealth(damage * -1);
		ShaderMaterial material = (ShaderMaterial)this.Material;
		material.SetShaderParameter("shake_intensity", 0.75f);
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		material.SetShaderParameter("shake_intensity", 0.0f);
	}

	public void ZeroHealthReached()
	{
		CallDeferred("addLogScene");
		GD.Print("Tree is destroyed");
		QueueFree();
	}

	private void addLogScene()
	{
		Item log_instance = _LogScene.Instantiate() as Item;
		RandomNumberGenerator rng = new RandomNumberGenerator();
		log_instance.GlobalPosition = GlobalPosition;
		log_instance.Velocity = Vector2.Right.Rotated(rng.RandfRange(-85, 85)) * rng.RandfRange(10, 100);
		GetParent().AddChild(log_instance);
	}
}
