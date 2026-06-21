using Godot;

public partial class Sword : HandItem
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    public int _Damage;


    /****************************** NODE VARIABLES ******************************/

    private StateMachine _StateMachine;
    private AnimationController _AnimationController;

    /****************************** OTHER VARIABLES ******************************/


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        _HitBoxComponent = FindChild("HitBoxComponent", recursive: true) as HitBoxComponent;
        // _StateMachine = GetNode<StateMachine>("StateMachine");

    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
