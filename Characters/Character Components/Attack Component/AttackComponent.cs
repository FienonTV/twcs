using System;
using Godot;

/// <summary>
/// Dependencies (What must be present for the class to work):
/// - Tool (including HitboxComponent)
///
/// The AttackHandler class processes attacks that are triggered by an InputHandler (player) or a behaviorClass of an NPC.
/// It controls when the HitboxComponent of the tool, which the character must carry in order to be able to perform attacks, is active and sends a signal to start the animation of the tool.
/// </summary>


public partial class AttackComponent : Node
{
    /****************************** EVENTS & SIGNALS ******************************/
    public event Action<Vector2> _StartAttackAnimation;

    /****************************** EXPORT VARIABLES ******************************/

    /****************************** NODE VARIABLES ******************************/
    private HitBoxComponent _HitBoxComponent;
    private Timer _HitBoxTimer;

    /****************************** OTHER VARIABLES ******************************/


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        _HitBoxTimer = FindChild("HitBoxTimer") as Timer;
        _HitBoxComponent =
            GetParent().FindChild("HitBoxComponent", recursive: true) as HitBoxComponent;
        _HitBoxTimer.Timeout += _HitBoxComponent.DeactivateHitBox;
        if (_HitBoxComponent != null)
            GD.Print("HitBoxComponent found");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    /****************************** EVENTHANDLER ******************************/
    public void OnAttackRequest()
    {
        _HitBoxComponent?.ActivateHitBox();
        _StartAttackAnimation.Invoke(GetParent<Player>()._CurrentLookingDirection);
        _HitBoxTimer.Start();
    }

    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
