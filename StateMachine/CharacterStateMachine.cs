using Godot;
using System.Collections.Generic;

public partial class CharacterStateMachine : Node2D
{
    protected CharacterState _CurrentState;
    protected CharacterState _PreviousState;

    [Export]
    protected CharacterState _DefaultState;

    [Export]
    private AnimationController AnimationController;

    [Export]
    private AnimationPlayer AnimationPlayer;

    public Vector2 CurrentDirection;

    public AnimationPlayer ActiveAnimationPlayer { get; private set; }
    public AnimationController ActiveAnimationController { get; private set; }

    private Dictionary<string, CharacterState> _StateRegistry = new Dictionary<string, CharacterState>();

    public override void _Ready()
    {
        BuildStateRegistry();
        ActiveAnimationController = ResolveAnimationController();
        ActiveAnimationPlayer = ActiveAnimationController?.AnimationPlayer ?? ResolveAnimationPlayer();
        _CurrentState = _DefaultState;
        _PreviousState = _CurrentState;
        CallDeferred(nameof(EnterDefaultState));
    }

    private void BuildStateRegistry()
    {
        _StateRegistry.Clear();
        foreach (Node child in GetChildren())
        {
            if (child is CharacterState state)
            {
                string key = state.StateKey;
                if (!_StateRegistry.ContainsKey(key))
                {
                    _StateRegistry.Add(key, state);
                }
            }
        }
    }

    protected virtual AnimationController ResolveAnimationController()
    {
        if (AnimationController != null)
        {
            return AnimationController;
        }
        if (Owner == null)
        {
            Logger.Error("CharacterStateMachine: Owner is null; cannot resolve AnimationController.");
            return null;
        }
        return Owner.GetNodeOrNull<AnimationController>("AnimationController");
    }

    protected virtual AnimationPlayer ResolveAnimationPlayer()
    {
        if (AnimationPlayer != null)
        {
            return AnimationPlayer;
        }
        if (Owner == null)
        {
            Logger.Error("CharacterStateMachine: Owner is null; cannot resolve AnimationPlayer.");
            return null;
        }
        return Owner.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
    }

    private void EnterDefaultState()
    {
        if (_CurrentState != null)
        {
            _CurrentState.Enter();
        }
        else
        {
            Logger.Error($"CharacterStateMachine: No default state assigned on '{Owner?.Name}'.");
        }
    }

    protected void ChangeState(string state)
    {
        if (!_StateRegistry.TryGetValue(state, out CharacterState newState))
        {
            Logger.Error($"CharacterStateMachine: State '{state}' not found.");
            return;
        }

        CharacterState oldState = _CurrentState;
        oldState?.Exit();
        _CurrentState = newState;
        _PreviousState = oldState;
        _CurrentState.Enter();
    }
}
