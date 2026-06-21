using Godot;

[GlobalClass]
public partial class HealItemEffectResource : ItemEffectResource
{

    [Export]
    int _HealAmount = 1;

    public override void Use(Character user)
    {
        if (user == null)
        {
            GD.PrintErr("HealItemEffectResource: No user provided.");
            return;
        }

        HealthComponent healthComponent = user.FindChild("HealthComponent", recursive: true) as HealthComponent;
        if (healthComponent == null)
        {
            GD.PrintErr("HealItemEffectResource: User has no HealthComponent.");
            return;
        }

        healthComponent.ChangeCurrentHealth(_HealAmount);
    }
}
