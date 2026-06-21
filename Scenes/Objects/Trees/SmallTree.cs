using Godot;

public partial class SmallTree : Sprite2D
{
    [Export]
    private HurtBoxComponent HurtBoxComponent;

    [Export]
    private HealthComponent HealthComponent;

    private PackedScene _LogScene = ResourceLoader.Load<PackedScene>(ResourcePaths.LogScene);

    public override void _Ready()
    {
        if (HurtBoxComponent == null)
        {
            HurtBoxComponent = FindChild("HurtboxComponent", recursive: true) as HurtBoxComponent;
        }
        if (HealthComponent == null)
        {
            HealthComponent = FindChild("HealthComponent", recursive: true) as HealthComponent;
        }

        if (HurtBoxComponent != null)
        {
            HurtBoxComponent.OnDamageReceived += ReceiveDamage;
        }
        else
        {
            Logger.Error("SmallTree: HurtBoxComponent not found.");
        }

        if (HealthComponent != null)
        {
            HealthComponent.HealthEmpty += ZeroHealthReached;
        }
        else
        {
            Logger.Error("SmallTree: HealthComponent not found.");
        }
    }

    public void ReceiveDamage(int damage)
    {
        HealthComponent?.ChangeHealth(damage * -1);
        if (Material is ShaderMaterial material)
        {
            material.SetShaderParameter("shake_intensity", 0.75f);
            Tween tween = CreateTween();
            tween.TweenInterval(0.5f);
            tween.TweenCallback(Callable.From(() => material.SetShaderParameter("shake_intensity", 0.0f)));
        }
    }

    public void ZeroHealthReached()
    {
        CallDeferred(nameof(addLogScene));
        Logger.Info("SmallTree: Tree destroyed.");
        QueueFree();
    }

    private void addLogScene()
    {
        Item logInstance = _LogScene.Instantiate() as Item;
        if (logInstance == null)
        {
            Logger.Error("SmallTree: Failed to instantiate log scene.");
            return;
        }
        RandomNumberGenerator rng = new RandomNumberGenerator();
        logInstance.GlobalPosition = GlobalPosition;
        logInstance.Velocity = Vector2.Right.Rotated(rng.RandfRange(-85, 85)) * rng.RandfRange(10, 100);
        GetParent().AddChild(logInstance);
    }
}
