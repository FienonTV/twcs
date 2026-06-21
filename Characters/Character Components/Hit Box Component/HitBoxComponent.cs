using Godot;
using System;

/// <summary>
/// Dependencies (What must be present for the class to work):
/// - Tool 
/// - Character that carries the Tool (inkl. AttackComponent)
/// 
/// Describes and defines the hitbox of a tool. If a tool has a HitboxComponent, it can inflict damage.
/// Reacts to signals from the attack component and activates/deactivates itself when these signals are received. 
/// Sends a signal itself when the hitbox is activated.
/// </summary>

public partial class HitBoxComponent : Area2D
{
    /****************************** EVENTS & SIGNALS ******************************/
    public event Action<int> OnHitboxActivated;




    /****************************** EXPORT VARIABLES ******************************/


    /****************************** NODE VARIABLES ******************************/
    public HandItem Tool;
    public Character OwnerCharacter;
    public CollisionShape2D CollisionShape;


    /****************************** OTHER VARIABLES ******************************/
    public bool IsActive = false;


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        CollisionShape = FindChild("CollisionShape2D", recursive: true) as CollisionShape2D;
        FindHandItemParent();
        FindCharacterParent();
    }

    public override void _Process(double delta)
    {
        if (OwnerCharacter == null || CollisionShape == null)
        {
            return;
        }
        ChangeCurrentHitboxPosition();
    }


    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/
    public void ActivateHitBox(Character owner = null)
    {
        if (owner != null)
        {
            OwnerCharacter = owner;
        }

        Logger.Debug("HitBoxComponent: HitBox activated.");
        IsActive = true;
        if (CollisionShape != null)
        {
            CollisionShape.Disabled = false;
        }
        OnHitboxActivated?.Invoke(Tool?._Damage ?? 0);
    }

    public void DeactivateHitBox()
    {
        Logger.Debug("HitBoxComponent: Hitbox deactivated.");
        IsActive = false;
        if (CollisionShape != null)
        {
            CollisionShape.Disabled = true;
        }
    }

    public void FindHandItemParent()
    {
        Node node = this;
        while (node != null)
        {
            if (node is HandItem handItem)
            {
                Tool = handItem;
                Logger.Debug("HitBoxComponent: HandItem parent found.");
                return;
            }
            node = node.GetParent();
        }
        Logger.Error("HitBoxComponent: No HandItem parent found.");
    }

    public void FindCharacterParent()
    {
        Node node = this;
        while (node != null)
        {
            if (node is Character character)
            {
                OwnerCharacter = character;
                Logger.Debug("HitBoxComponent: Character parent found.");
                return;
            }
            node = node.GetParent();
        }
        Logger.Error("HitBoxComponent: No Character parent found.");
    }

    private void ChangeCurrentHitboxPosition()
    {
        if (OwnerCharacter == null)
        {
            return;
        }
        Position = OwnerCharacter.CurrentLookingDirection * 20;
        Rotation = OwnerCharacter.CurrentLookingDirection.Angle();
    }

    /****************************** GETTER & SETTER METHODS ******************************/
}
