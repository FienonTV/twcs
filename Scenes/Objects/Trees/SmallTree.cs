using Godot;

public partial class SmallTree : Sprite2D
{
	HurtBoxComponent _HurtBoxComponent;
	HealthComponent _HealthComponent;

	PackedScene _LogScene = ResourceLoader.Load<PackedScene>(ResourcePaths.LogScene);

	public override void _Ready()
	{
		_HurtBoxComponent = FindChild("HurtboxComponent", recursive: true) as HurtBoxComponent;
		_HealthComponent = FindChild("HealthComponent", recursive: true) as HealthComponent;

		if (_HurtBoxComponent != null)
		{
			_HurtBoxComponent.OnDamageReceived += ReceiveDamage;
		}

		if (_HealthComponent != null)
		{
			_HealthComponent._HealthEmpty += ZeroHealthReached;
		}
	}

	public void ReceiveDamage(int damage)
	{
		_HealthComponent?.ChangeHealth(damage * -1);
		ShaderMaterial material = (ShaderMaterial)this.Material;
		material.SetShaderParameter("shake_intensity", 0.75f);

		Tween tween = CreateTween();
		tween.TweenInterval(0.5f);
		tween.TweenCallback(Callable.From(() => material.SetShaderParameter("shake_intensity", 0.0f)));
	}

	public void ZeroHealthReached()
	{
		CallDeferred("addLogScene");
		Logger.Info("SmallTree: Tree destroyed.");
		QueueFree();
	}

	private void addLogScene()
	{
		Item logInstance = _LogScene.Instantiate() as Item;
		RandomNumberGenerator rng = new RandomNumberGenerator();
		logInstance.GlobalPosition = GlobalPosition;
		logInstance.Velocity = Vector2.Right.Rotated(rng.RandfRange(-85, 85)) * rng.RandfRange(10, 100);
		GetParent().AddChild(logInstance);
	}
}
