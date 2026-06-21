using Godot;
using System;

/// <summary>
/// Dependencies (What must be present for the class to work):
/// - HealthComponent (Works without a HealtComponent, but makes no sense if no damage is to be processed.) 
/// 
/// Component that decides whether an object is attackable or not. Stores all hitboxes in the hurtbox in a list using AreaEntered and AreaExited signals.
/// If one of these hitboxes sends a HitboxActivated signal, the hurtbox itself sends two signals.
/// One to start visual effects and one to apply the damage taken.
/// </summary>

public partial class HurtBoxComponent : Area2D
{
    /****************************** EVENTS & SIGNALS ******************************/
    public event Action<int> OnDamageReceived;


    /****************************** EXPORT VARIABLES ******************************/

    [Export]
    Godot.Collections.Array<DataTypes.HandItemsTypes> EffectiveItems = new Godot.Collections.Array<DataTypes.HandItemsTypes>();

    [Export]
    private Timer CooldownTimer;


    /****************************** NODE VARIABLES ******************************/



    /****************************** OTHER VARIABLES ******************************/
    private bool _CanGetDamage = true;


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        if (CooldownTimer == null)
        {
            CooldownTimer = FindChild("CooldownTimer") as Timer;
        }
        AreaEntered += OnAreaEntered;
    }

    /****************************** EVENTHANDLER ******************************/
    private void OnAreaEntered(Area2D hitbox)
    {
        HitBoxComponent hitBoxComponent = hitbox as HitBoxComponent;
        if (hitBoxComponent == null || hitBoxComponent.Tool == null)
        {
            return;
        }

        if (hitBoxComponent.Owner is HandItem)
        {
            if (EffectiveItems.Contains(hitBoxComponent.Tool.HandItemCategory))
            {
                OnDamageReceived?.Invoke(hitBoxComponent.Tool.Damage);
            }
        }
    }


    /****************************** OTHER METHODS ******************************/
    private void DeactivateCooldown()
    {
        _CanGetDamage = true;
    }



    /****************************** GETTER & SETTER METHODS ******************************/
}
