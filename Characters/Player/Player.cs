using Godot;
using System;

public partial class Player : Character
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    string _Name = "Player";

    /****************************** NODE VARIABLES ******************************/
    private PlayerMovementComponent _PlayerMovementComponent;
    private AttackComponent _AttackComponent;
    private InputHandler _InputHandler;
    private HitBoxComponent _HitBoxComponent;

    private PlayerInteractionComponents _PlayerInteractionComponents;

    private HandItem _CurrentHandItem;



    /****************************** OTHER VARIABLES ******************************/

    public InventoryDataResource _INVENTORY_DATA = ResourceLoader.Load<InventoryDataResource>("res://UI/Inventory/Player_Inventory.tres") as InventoryDataResource;

    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        AddToGroup("Player");
        GameManager.RegisterPlayer(this);

        _HealthComponent = FindChild("HealthComponent", true) as HealthComponent;
        if (_HealthComponent == null)
        {
            GD.PrintErr("Player: Can't find HealthComponent");
        }

        _HitBoxComponent = FindChild("HitBoxComponent", true) as HitBoxComponent;

        _InputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");
        if (_InputHandler == null)
        {
            GD.PrintErr("Player: InputHandler autoload not found.");
        }

        _PlayerInteractionComponents = GetNodeOrNull<PlayerInteractionComponents>(
            "Interaction Components"
        );
        if (_PlayerInteractionComponents == null)
        {
            GD.PrintErr("Player: Interaction Components not found.");
        }

        _AttackComponent = GetNodeOrNull<AttackComponent>("AttackComponent");
        if (_AttackComponent == null)
        {
            GD.PrintErr("Player: AttackComponent not found.");
        }

        _PlayerMovementComponent = FindChild("MovementComponent") as PlayerMovementComponent;
        if (_PlayerMovementComponent == null)
        {
            GD.PrintErr("Player: MovementComponent not found.");
        }

        Node toolNode = GetNodeOrNull("Tool");
        if (toolNode != null && toolNode.GetChildCount() > 0)
        {
            _CurrentHandItem = toolNode.GetChild(0) as HandItem;
        }

        if (_CurrentHandItem == null)
        {
            GD.PrintErr("Player: No HandItem equipped under Tool.");
        }

        _HealthComponent?.SetHealth(_HealthComponent.GetMaxHealth());

        if (_PlayerMovementComponent != null && _InputHandler != null)
        {
            _InputHandler._OnMoveInput += _PlayerMovementComponent.HandleMovement;
        }
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
