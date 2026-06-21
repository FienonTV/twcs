using Godot;

public partial class Sword : HandItem
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/


    /****************************** NODE VARIABLES ******************************/

    private AnimationController _AnimationController;

    /****************************** OTHER VARIABLES ******************************/


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        HitBoxComponent = FindChild("HitBoxComponent", recursive: true) as HitBoxComponent;
        if (HitBoxComponent == null)
        {
            Logger.Error("Sword: No HitBoxComponent found.");
        }
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
