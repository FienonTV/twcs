using Godot;

public partial class Sword : HandItem
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    public int _Damage;


    /****************************** NODE VARIABLES ******************************/

    private AnimationController _AnimationController;

    /****************************** OTHER VARIABLES ******************************/


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        _HitBoxComponent = FindChild("HitBoxComponent", recursive: true) as HitBoxComponent;
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
