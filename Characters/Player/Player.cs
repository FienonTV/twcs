using Godot;
using System;

public partial class Player : Character
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    string DisplayName = "Player";

    /****************************** NODE VARIABLES ******************************/
    private PlayerMovementComponent _PlayerMovementComponent;
    private AttackComponent _AttackComponent;
    private InputHandler _InputHandler;

    private PlayerInteractionComponents _PlayerInteractionComponents;

    private HandItem _CurrentHandItem;

    private GameManager _GameManager;


    /****************************** OTHER VARIABLES ******************************/

    public InventoryDataResource InventoryData = ResourceLoader.Load<InventoryDataResource>(ResourcePaths.InventoryTres) as InventoryDataResource;

    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        AddToGroup("Player");

        _GameManager = GetNodeOrNull<GameManager>("/root/GameManager");
        _GameManager?.RegisterPlayer(this);

        HealthComponent = FindChild("HealthComponent", true) as HealthComponent;
        if (HealthComponent == null)
        {
            Logger.Error("Player: Can't find HealthComponent");
        }

        _InputHandler = GetNodeOrNull<InputHandler>("/root/InputHandler");
        if (_InputHandler == null)
        {
            Logger.Error("Player: InputHandler autoload not found.");
        }

        _PlayerInteractionComponents = GetNodeOrNull<PlayerInteractionComponents>(
            "Interaction Components"
        );
        if (_PlayerInteractionComponents == null)
        {
            Logger.Error("Player: Interaction Components not found.");
        }

        _AttackComponent = GetNodeOrNull<AttackComponent>("AttackComponent");
        if (_AttackComponent == null)
        {
            Logger.Error("Player: AttackComponent not found.");
        }

        _PlayerMovementComponent = FindChild("MovementComponent") as PlayerMovementComponent;
        if (_PlayerMovementComponent == null)
        {
            Logger.Error("Player: MovementComponent not found.");
        }

        EquipHandItemFromToolNode();

        HealthComponent?.SetHealth(HealthComponent.GetMaxHealth());

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
            Logger.Error("Player: No Tool node found.");
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

        Logger.Error("Player: No HandItem equipped under Tool.");
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
