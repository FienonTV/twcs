using Godot;

public partial class Character : CharacterBody2D
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export] public NavigationAgent2D navigationAgent2D;

    /****************************** NODE VARIABLES ******************************/
    protected CharacterStateMachine StateMachine;
    protected AnimationController _AnimationController;
    public HealthComponent HealthComponent;
    protected HurtBoxComponent _HurtBoxComponent;

    /****************************** OTHER VARIABLES ******************************/
    public Vector2 CurrentLookingDirection = Vector2.Down;

    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        HealthComponent = FindChild("HealthComponent", true) as HealthComponent;
        if (HealthComponent == null)
        {
            Logger.Error("Character: Can't find HealthComponent");
        }
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
