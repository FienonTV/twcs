using Godot;

public partial class Character : CharacterBody2D
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    public NavigationAgent2D NavigationAgent2D;

    [Export]
    public HealthComponent HealthComponent;

    /****************************** NODE VARIABLES ******************************/
    protected CharacterStateMachine StateMachine;
    protected AnimationController AnimationController;
    protected HurtBoxComponent HurtBoxComponent;

    /****************************** OTHER VARIABLES ******************************/
    public Vector2 CurrentLookingDirection = Vector2.Down;

    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        if (HealthComponent == null)
        {
            HealthComponent = FindChild("HealthComponent", true) as HealthComponent;
            if (HealthComponent == null)
            {
                Logger.Error($"Character: Can't find HealthComponent on '{Name}'.");
            }
        }
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
