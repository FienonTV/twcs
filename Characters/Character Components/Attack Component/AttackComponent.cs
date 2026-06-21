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
    private HitBoxComponent HitBoxComponent;

    [Export]
    private Timer HitBoxTimer;

    /****************************** OTHER VARIABLES ******************************/


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        if (HitBoxComponent == null)
        {
            // Fallback: search in the tool node if the character exposes one.
            Node toolNode = GetParent()?.GetNodeOrNull("Tool");
            if (toolNode != null)
            {
                HitBoxComponent = toolNode.FindChild("HitBoxComponent", recursive: true) as HitBoxComponent;
            }
        }

        if (HitBoxTimer == null)
        {
            HitBoxTimer = FindChild("HitBoxTimer") as Timer;
        }

        if (HitBoxComponent != null)
        {
            if (HitBoxTimer != null)
            {
                HitBoxTimer.Timeout += HitBoxComponent.DeactivateHitBox;
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

        HitBoxComponent?.ActivateHitBox(ownerCharacter);
        Vector2 direction = ownerCharacter.CurrentLookingDirection;
        StartAttackAnimation?.Invoke(direction);
        HitBoxTimer?.Start();
    }

    /****************************** OTHER METHODS ******************************/


    /****************************** GETTER & SETTER METHODS ******************************/
}
