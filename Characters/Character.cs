using Godot;

public partial class Character : CharacterBody2D
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export] public NavigationAgent2D navigationAgent2D;

    /****************************** NODE VARIABLES ******************************/
    protected newStateMachine _StateMachine;
    protected AnimationController _AnimationController;
    public HealthComponent _HealthComponent;
    protected HurtBoxComponent _HurtBoxComponent;

    /****************************** OTHER VARIABLES ******************************/
    public Vector2 _CurrentLookingDirection = Vector2.Down;

    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        // _StateMachine = GetNode<newStateMachine>("StateMachine");
        //_AnimationController = GetNode<AnimationController>("AnimationController");
        _HealthComponent = FindChild("HealthComponent", true) as HealthComponent;
        if (_HealthComponent == null)
        {
            GD.PrintErr("Can't find HealtComponent");
        }
        // _HurtBoxComponent = GetNode<HurtBoxComponent>("HurtBoxComponent");




        //_HurtBoxComponent.OnDamageRecived += _StateMachine.StartStateEffect;
        //_HurtBoxComponent.ReduceHealth += _HealthComponent.ChangeHealth;
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
