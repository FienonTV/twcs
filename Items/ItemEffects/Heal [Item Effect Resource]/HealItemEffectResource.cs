using Godot;

[GlobalClass]
public partial class HealItemEffectResource : ItemEffectResource
{
    [Export]
    private int HealAmount = 1;

    public override void Use(Character user)
    {
        if (user == null)
        {
            Logger.Error("HealItemEffectResource: No user provided.");
            return;
        }

        HealthComponent healthComponent = user.HealthComponent;
        if (healthComponent == null)
        {
            Logger.Error("HealItemEffectResource: User has no HealthComponent.");
            return;
        }

        healthComponent.ChangeCurrentHealth(HealAmount);
    }
}
