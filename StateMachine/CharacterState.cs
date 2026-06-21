using Godot;
using System.Collections.Generic;
public partial class CharacterState : Node2D
{
    protected CharacterStateMachine _StateMachine;

    protected Vector2 _LastAnimationDirection = Vector2.Down;
    public override void _Ready()
    {
        SetPhysicsProcess(false);
        _StateMachine = GetParent<CharacterStateMachine>();
    }

    public virtual void Enter()
    {
        SetPhysicsProcess(true);
    }
    public virtual void Exit()
    {
        SetPhysicsProcess(false);
    }

    public void Transition()
    {

    }

    public override void _PhysicsProcess(double delta)
    {

    }

    // This method starts the appropriate animation based on the movement direction
    public virtual void StartAnimation(string animationName)
    {
        // Runde die Richtung auf den nächsten ganzzahligen Vektor
        Vector2 roundedDirection = new Vector2(
            Mathf.Round(_StateMachine._CurrentDirection.X),
            Mathf.Round(_StateMachine._CurrentDirection.Y)
        );
        // Try to get the animation name based on the movement direction
        if (DirectionForAnimation.TryGetValue(roundedDirection, out string animationDirection))
        {
            // Play the corresponding animation
            if (_StateMachine._AnimationPlayer.HasAnimation(animationName + animationDirection))
            {
                _StateMachine._AnimationPlayer.Play(animationName + animationDirection);
                // Store the last animation direction
                _LastAnimationDirection = _StateMachine._CurrentDirection;
                var owner = _StateMachine.Owner as Character;
                if (owner != null)
                {
                    owner._CurrentLookingDirection = _StateMachine._CurrentDirection;
                }

            }
            else
            {
                PlayLastAnimation(animationName);
            }
        }
        else
        {
            PlayLastAnimation(animationName);
        }

    }

    //TODO: CHECK IF THE COMMENTED LINES ARE NEEDED ANYMORE
    private void PlayLastAnimation(string animationName)
    {
        // If the direction is not found, play the last animation direction
        if (_LastAnimationDirection == Vector2.Right)
        {

            _StateMachine._AnimationPlayer.Play(animationName + "right");
            //_StateMachine._CharacterParent._CurrentLookingDirection = Vector2.Right;
        }
        else if (_LastAnimationDirection == Vector2.Left)
        {
            _StateMachine._AnimationPlayer.Play(animationName + "left");
            //_StateMachine._CharacterParent._CurrentLookingDirection = Vector2.Left;
        }
        else if (_LastAnimationDirection == Vector2.Up)
        {
            if (_StateMachine._AnimationPlayer.HasAnimation(animationName + "up"))
            {
                _StateMachine._AnimationPlayer.Play(animationName + "up");
                //_StateMachine._CharacterParent._CurrentLookingDirection = Vector2.Up;
            }
        }
        else if (_LastAnimationDirection == Vector2.Down)
        {
            if (_StateMachine._AnimationPlayer.HasAnimation(animationName + "down"))
            {
                _StateMachine._AnimationPlayer.Play(animationName + "down");
                //_StateMachine._CharacterParent._CurrentLookingDirection = Vector2.Down;
            }
        }
    }

    protected Dictionary<Vector2, string> DirectionForAnimation = new Dictionary<Vector2, string>
    {
        { new Vector2(1,0), "right" },
        { new Vector2(-1,0), "left" },
        { new Vector2(0,1), "down" },
        { new Vector2(0,-1), "up" }
    };
}