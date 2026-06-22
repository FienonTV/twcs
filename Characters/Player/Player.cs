using Godot;
using System;

public partial class Player : Character
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    string DisplayName = "Player";

    [Export]
    private PlayerMovementComponent MovementComponent;

    [Export]
    private AttackComponent AttackComponent;

    [Export]
    private InputHandler InputHandler;

    [Export]
    private PlayerInteractionComponents InteractionComponents;

    [Export]
    private Node ToolNode;

    /****************************** NODE VARIABLES ******************************/
    private HandItem _CurrentHandItem;

    private GameManager _GameManager;


    /****************************** OTHER VARIABLES ******************************/

    [Export]
    public InventoryDataResource InventoryData;

    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        AddToGroup("Player");

        _GameManager = Services.Get<GameManager>();
        _GameManager?.RegisterPlayer(this);

        if (HealthComponent == null)
        {
            HealthComponent = FindChild("HealthComponent", true) as HealthComponent;
        }
        if (HealthComponent == null)
        {
            Logger.Error("Player: Can't find HealthComponent");
        }

        if (InputHandler == null)
        {
            InputHandler = Services.Get<InputHandler>();
        }

        if (MovementComponent == null)
        {
            MovementComponent = FindChild("MovementComponent") as PlayerMovementComponent;
        }
        if (MovementComponent == null)
        {
            Logger.Error("Player: MovementComponent not found.");
        }

        if (AttackComponent == null)
        {
            AttackComponent = FindChild("AttackComponent") as AttackComponent;
        }
        if (AttackComponent == null)
        {
            Logger.Error("Player: AttackComponent not found.");
        }

        if (InteractionComponents == null)
        {
            InteractionComponents = FindChild("Interaction Components") as PlayerInteractionComponents;
        }
        if (InteractionComponents == null)
        {
            Logger.Error("Player: Interaction Components not found.");
        }

        EquipHandItemFromToolNode();

        HealthComponent?.SetHealth(HealthComponent.GetMaxHealth());

        if (MovementComponent != null && InputHandler != null)
        {
            InputHandler._OnMoveInput += MovementComponent.HandleMovement;
        }

        InventoryMenu inventoryMenu = Services.Get<InventoryMenu>();
        if (inventoryMenu != null)
        {
            inventoryMenu.CurrentUser = this;
            if (InventoryData == null)
            {
                InventoryData = new InventoryDataResource();
                InventoryData.Slots = new SlotDataResource[28];
            }

            inventoryMenu.InventoryData = InventoryData;
            InventoryUI inventoryUI = inventoryMenu.GetNodeOrNull<InventoryUI>("Control/PanelContainer/GridContainer");
            if (inventoryUI != null)
            {
                inventoryUI._Data = InventoryData;
            }
        }
    }

    private void EquipHandItemFromToolNode()
    {
        if (ToolNode == null)
        {
            Logger.Error("Player: No Tool node assigned.");
            return;
        }

        foreach (Node child in ToolNode.GetChildren())
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
