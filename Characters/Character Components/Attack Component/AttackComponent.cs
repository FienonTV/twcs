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
    public event Action<Vector2> StartAttackAnimation;

    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    private HitBoxComponent _HitBoxComponent;

    /****************************** NODE VARIABLES ******************************/
    private Timer _HitBoxTimer;

    /****************************** OTHER VARIABLES ******************************/


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        _HitBoxTimer = FindChild("HitBoxTimer") as Timer;

        if (_HitBoxComponent == null)
        {
            // Fallback: search in the tool node if the character exposes one.
            Node toolNode = GetParent()?.GetNodeOrNull("Tool");
            if (toolNode != null)
            {
                _HitBoxComponent = toolNode.FindChild("HitBoxComponent", recursive: true) as HitBoxComponent;
            }
        }

        if (_HitBoxComponent != null)
        {
            if (_HitBoxTimer != null)
            {
                _HitBoxTimer.Timeout += _HitBoxComponent.DeactivateHitBox;
            }
            Logger.Debug("AttackComponent: HitBoxComponent found.");
        }
        else
        {
            Logger.Error("AttackComponent: No HitBoxComponent found.");
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    /****************************** EVENTHANDLER ******************************/
    public void OnAttackRequest()
    {
        Character ownerCharacter = GetParent<Character>();
        if (ownerCharacter == null)
        {
            Logger.Error("AttackComponent: Owner is not a Character.");
            return;
        }

        _HitBoxComponent?.ActivateHitBox(ownerCharacter);
        Vector2 direction = ownerCharacter.CurrentLookingDirection;
        StartAttackAnimation?.Invoke(direction);
        _HitBoxTimer?.Start();
    }

    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
