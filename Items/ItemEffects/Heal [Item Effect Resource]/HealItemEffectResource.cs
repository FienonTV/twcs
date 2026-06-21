using Godot;

[GlobalClass]
public partial class HealItemEffectResource : ItemEffectResource
{

    [Export]
    int _HealAmount = 1;

    public override void Use()
    {
        Player targetPlayer = GameManager.getPlayer();
        if (targetPlayer == null)
        {
            GD.PrintErr("HealItemEffectResource: No target player found.");
            return;
        }

        if (targetPlayer._HealthComponent == null)
        {
            GD.PrintErr("HealItemEffectResource: Target player has no HealthComponent.");
            return;
        }

        targetPlayer._HealthComponent.ChangeCurrentHealth(_HealAmount);
    }
}
