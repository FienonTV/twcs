using Godot;

public partial class Character : CharacterBody2D
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export] public NavigationAgent2D navigationAgent2D;

    /****************************** NODE VARIABLES ******************************/
    protected CharacterStateMachine _StateMachine;
    protected AnimationController _AnimationController;
    public HealthComponent _HealthComponent;
    protected HurtBoxComponent _HurtBoxComponent;

    /****************************** OTHER VARIABLES ******************************/
    public Vector2 _CurrentLookingDirection = Vector2.Down;

    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        _HealthComponent = FindChild("HealthComponent", true) as HealthComponent;
        if (_HealthComponent == null)
        {
            GD.PrintErr("Character: Can't find HealthComponent");
        }
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
