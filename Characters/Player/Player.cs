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

        EquipHandItemFromToolNode();

        _HealthComponent?.SetHealth(_HealthComponent.GetMaxHealth());

        if (_PlayerMovementComponent != null && _InputHandler != null)
        {
            _InputHandler._OnMoveInput += _PlayerMovementComponent.HandleMovement;
        }
    }

    private void EquipHandItemFromToolNode()
    {
        Node toolNode = GetNodeOrNull("Tool");
        if (toolNode == null)
        {
            GD.PrintErr("Player: No Tool node found.");
            return;
        }

        foreach (Node child in toolNode.GetChildren())
        {
            if (child is HandItem handItem)
            {
                _CurrentHandItem = handItem;
                return;
            }
        }

        GD.PrintErr("Player: No HandItem equipped under Tool.");
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
