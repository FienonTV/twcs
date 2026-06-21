using Godot;

[GlobalClass]
public partial class HealItemEffectResource : ItemEffectResource
{

    [Export]
    int _HealAmount = 1;

    public override void Use()
    {

        if (GameManager.getPlayer() == null)
        {
            GD.PrintErr("Cant find Global Player");
        }
        if (GameManager.getPlayer()._HealthComponent == null)
        {
            GD.PrintErr("Cant find Global Player HealthComponent");
        }
        else
        {
            GameManager.getPlayer()._HealthComponent.ChangeCurrentHealth(_HealAmount);
        }
    }
}