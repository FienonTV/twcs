using Godot;
using System.Collections.Generic;
using System.Linq;


public partial class StateMachine : Node
{
    public State currentState;
    private Dictionary<string, State> states = new Dictionary<string, State>();
    public AnimationController _AnimationController;
    public Vector2 _CurrentMovementDirection;

    public Character _CharacterParent;

    //Initializing all State Node Childs of the StateMachine and add it into the Directory
    public override void _Ready()
    {
        foreach (State state in GetChildren().OfType<State>())
        {
            states[state.Name] = state;
            state._StateMachine = this;
        }
        FindCharacterParent();
        _AnimationController = GetParent().GetNode<AnimationController>("AnimationController") as AnimationController;

        if (_AnimationController != null)
        {
            GD.Print("Animation Controller TRUE");
        }
        else { GD.Print("Animation Controller FALSE"); }
    }

    public void TransitionTo(string stateName, Vector2 direction)
    {
        GD.Print("I am here");
        if (states.ContainsKey(stateName))
        {
            currentState = states[stateName];
            if (_CharacterParent._CurrentLookingDirection != Vector2.Zero)
                GD.Print("Transitioning to " + stateName + " with direction " + direction.Round());
            currentState.Enter(direction.Round(), _AnimationController);
        }
        else
        {
            GD.Print("State not Found");
        }
    }

    public void ReturnToIdle()
    {
        if (currentState is not IdleState)
        {
            GD.Print("Returning to Idle");
            TransitionTo("IdleState", _CharacterParent._CurrentLookingDirection);
        }
    }

    public void FindCharacterParent()
    {
        Node node = this;
        while (node != null) // Solange ein Parent existiert
        {
            if (node is Character character) // Prüfen, ob es vom Typ Character (oder abgeleitet) ist
            {
                _CharacterParent = character;
                GD.Print("Parent gefunden"); // Charakter gefunden, zurückgeben
            }
            node = node.GetParent(); // Zum nächsten Parent wechseln
        }
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void StartStateEffect(string effectName)
    {
        _AnimationController.PlayEffect(effectName);
    }
}