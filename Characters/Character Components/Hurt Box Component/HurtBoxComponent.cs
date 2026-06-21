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
/// 

public partial class HurtBoxComponent : Area2D
{
    /****************************** EVENTS & SIGNALS ******************************/
    public event Action<int> _OnDamageRecived;


    /****************************** EXPORT VARIABLES ******************************/

    [Export]
    Godot.Collections.Array<data_types.HandItemsTypes> _EffectiveItems = new Godot.Collections.Array<data_types.HandItemsTypes>();

    /****************************** NODE VARIABLES ******************************/

    private Timer _CoolDownTimer;


    /****************************** OTHER VARIABLES ******************************/
    private bool _CanGetDamage = true;


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        _CoolDownTimer = FindChild("CooldownTimer") as Timer;
        AreaEntered += OnAreaEntered;
    }

    /****************************** EVENTHANDLER ******************************/
    private void OnAreaEntered(Area2D hitbox)
    {
        HitBoxComponent hitBoxComponent = hitbox as HitBoxComponent;

        if (hitBoxComponent.Owner is HandItem)
        {
            if (_EffectiveItems.Contains(hitBoxComponent._Tool._HandItemCategory))
            {
                _OnDamageRecived.Invoke(hitBoxComponent._Tool._Damage);
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
