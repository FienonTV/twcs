using System;
using Godot;

public partial class Player : Character
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    String _Name = "Player";

    /****************************** NODE VARIABLES ******************************/
    private PlayerMovementComponent _PlayerMovementComponent;
    private AttackComponent _AttackComponent;
    private InputHandler _InputHandler;
    private HitBoxComponent _HitBoxComponent;

    private PlayerInteractionComponents _PlayerInteractionComponents;
    private Sword _Tool;

    private HandItem _CurrentHandItem;



    /****************************** OTHER VARIABLES ******************************/

    public InventoryDataResource _INVENTORY_DATA = ResourceLoader.Load<InventoryDataResource>("res://UI/Inventory/Player_Inventory.tres") as InventoryDataResource;

    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        AddToGroup("Player");
        _HealthComponent = FindChild("HealthComponent", true) as HealthComponent;
        if (_HealthComponent == null)
        {
            GD.PrintErr("Can't find HealtComponent");
        }

        _HitBoxComponent = (HitBoxComponent)FindChild("HitBoxComponent", true);
        _InputHandler = GetNode("/root/InputHandler") as InputHandler;
        _PlayerInteractionComponents = GetNode<PlayerInteractionComponents>(
            "Interaction Components"
        );
        _AttackComponent = GetNode<AttackComponent>("AttackComponent");
        _PlayerMovementComponent = FindChild("MovementComponent") as PlayerMovementComponent;


        _Tool = GetNode("Tool").GetChild(0) as Sword;
        _HealthComponent.SetHealth(_HealthComponent.GetMaxHealth());
       /* if (_CurrentHandItem._HandItemCategory is data_types.HandItemsTypes.MeeleeWeapon)
        {

        }*/
        _InputHandler._OnMoveInput += _PlayerMovementComponent.HandleMovement;
        //_InputHandler._OnAttackInput += _AttackComponent.OnAttackRequest;
        //_InputHandler._NoMovement += _StateMachine.ReturnToIdle;
        //_InputHandler._OnInteractionInput += _PlayerInteractionComponents.OnInterActionExecute;
        //_PlayerMovementComponent._OnMovingPerformed += _StateMachine.TransitionTo;
        //_AttackComponent._StartAttackAnimation += _Tool.OnUse;
    }



    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
