public partial class Enemy : Character
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/


    /****************************** NODE VARIABLES ******************************/
    public NPCInputHandler _NPCInputHandler;
    //public CharacterMovementComponent _CharacterMovementComponent;

    public PlayerMovementComponent _PlayerMovementComponent;


    /****************************** OTHER VARIABLES ******************************/


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        CallDeferred("initialize");
        base._Ready();

    }
    public async void initialize()
    {
        await ToSignal(GetTree(), "physics_frame");

        //_PlayerMovementComponent = GetNode<PlayerMovementComponent>("MovementComponent");
        // _PlayerMovementComponent._OnMovingPerformed += _StateMachine.TransitionTo;

        //Find Inputhandler
        //_NPCInputHandler = GetNode<NPCInputHandler>("NPCInputHandler");

        //Make sure that the signal is only connected when the NPCInputHandler has been successfully initialized
        // _NPCInputHandler._OnMoveRequest += _PlayerMovementComponent.HandleMovement;
    }




    //_NPCInputHandler.UpdateInput(this.GlobalPosition, Vector2.Zero);
    // _CharacterMovementComponent.HandleMovement();


    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}