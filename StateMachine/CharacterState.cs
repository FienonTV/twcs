using Godot;
using System.Collections.Generic;

public partial class CharacterState : Node2D
{
    protected CharacterStateMachine StateMachine;

    protected Vector2 _LastAnimationDirection = Vector2.Down;

    public virtual string StateKey => Name;

    public override void _Ready()
    {
        SetPhysicsProcess(false);
        StateMachine = GetParent<CharacterStateMachine>();
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

    public virtual void StartAnimation(string animationName)
    {
        Vector2 roundedDirection = new Vector2(
            Mathf.Round(StateMachine.CurrentDirection.X),
            Mathf.Round(StateMachine.CurrentDirection.Y)
        );

        AnimationPlayer player = StateMachine.ActiveAnimationPlayer;
        if (player == null)
        {
            return;
        }

        if (DirectionForAnimation.TryGetValue(roundedDirection, out string animationDirection))
        {
            if (player.HasAnimation(animationName + animationDirection))
            {
                player.Play(animationName + animationDirection);
                _LastAnimationDirection = StateMachine.CurrentDirection;
                var owner = StateMachine.Owner as Character;
                if (owner != null)
                {
                    owner.CurrentLookingDirection = StateMachine.CurrentDirection;
                }
            }
            else
            {
                PlayLastAnimation(animationName, player);
            }
        }
        else
        {
            PlayLastAnimation(animationName, player);
        }
    }

    private void PlayLastAnimation(string animationName, AnimationPlayer player)
    {
        if (_LastAnimationDirection == Vector2.Right)
        {
            player.Play(animationName + "right");
        }
        else if (_LastAnimationDirection == Vector2.Left)
        {
            player.Play(animationName + "left");
        }
        else if (_LastAnimationDirection == Vector2.Up && player.HasAnimation(animationName + "up"))
        {
            player.Play(animationName + "up");
        }
        else if (_LastAnimationDirection == Vector2.Down && player.HasAnimation(animationName + "down"))
        {
            player.Play(animationName + "down");
        }
    }

    protected Dictionary<Vector2, string> DirectionForAnimation = new Dictionary<Vector2, string>
    {
        { new Vector2(1, 0), "right" },
        { new Vector2(-1, 0), "left" },
        { new Vector2(0, 1), "down" },
        { new Vector2(0, -1), "up" }
    };
}
