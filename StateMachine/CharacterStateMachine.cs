using Godot;
using System.Collections.Generic;

public partial class CharacterStateMachine : Node2D
{
    protected CharacterState _CurrentState;
    protected CharacterState _PreviousState;

    [Export]
    protected CharacterState _DefaultState;

    public Vector2 _CurrentDirection;
    public AnimationPlayer _AnimationPlayer;

    protected AnimationController _AnimationController;

    private Dictionary<string, CharacterState> _StateRegistry = new Dictionary<string, CharacterState>();

    public override void _Ready()
    {
        BuildStateRegistry();
        _AnimationPlayer = ResolveAnimationPlayer();
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
                string key = child.Name;
                if (!_StateRegistry.ContainsKey(key))
                {
                    _StateRegistry.Add(key, state);
                }
            }
        }
    }

    protected virtual AnimationPlayer ResolveAnimationPlayer()
    {
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
            GD.PrintErr("CharacterStateMachine: No default state assigned on " + Owner?.Name);
        }
    }

    protected void ChangeState(string state)
    {
        if (!_StateRegistry.TryGetValue(state, out CharacterState newState))
        {
            GD.PrintErr("State '" + state + "' not found");
            return;
        }

        CharacterState oldState = _CurrentState;
        oldState?.Exit();
        _CurrentState = newState;
        _PreviousState = oldState;
        _CurrentState.Enter();
    }
}
