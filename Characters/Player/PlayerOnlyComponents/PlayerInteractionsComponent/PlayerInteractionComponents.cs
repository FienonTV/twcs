using Godot;
using System.Collections.Generic;

public partial class PlayerInteractionComponents : Node2D
{
    Player _CharacterParent;
    Interaction_Area InteractionArea;
    CollisionShape2D _InteractionAreaCollisionShape;

    private Vector2 _InteractionOffset = new Vector2(15, 0);

    Label InteractLabel;

    bool _isLoaded = false;

    private List<Interaction_Area> all_interactions = new List<Interaction_Area>();


    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        await ToSignal(GetTree(), "process_frame");
        GD.Print(this.GetParent().Name);
        _CharacterParent = FindParent("Player") as Player;
        if (_CharacterParent == null)
        {
            GD.Print("CharacterParent is null");
        }
        else
        {
            GD.Print("CharacterParent is not null");
        }
        InteractionArea = Owner.FindChild("InteractionArea", true) as Interaction_Area;

        if (InteractionArea == null)
        {
            GD.Print("InteractionArea is null");
        }
        _InteractionAreaCollisionShape = InteractionArea.GetNode<CollisionShape2D>("CollisionShape2D");
        if (_InteractionAreaCollisionShape == null)
        {
            GD.Print("= null");
        }
        InteractLabel = FindChild("InteractLabel", recursive: true) as Label;


        InteractionArea.AreaEntered += on_interaction_area_entered;
        InteractionArea.AreaExited += on_interaction_area_exited;

        updateInteractions();
        _isLoaded = true;


    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_isLoaded)
        {
            ChangeCurrentInteractionCollisionShapeDirection();

            if (Input.IsActionJustPressed("interact"))
            {

                executeInteraction();
            }
        }
    }


    ////////////Interaction Methods////////////
    private void on_interaction_area_entered(Area2D area)
    {

        if (area is Interaction_Area interactable)
        {
            all_interactions.Insert(0, interactable);
            updateInteractions();
        }

    }

    private void on_interaction_area_exited(Area2D area)
    {
        if (area is Interaction_Area interactable)
        {
            all_interactions.Remove(interactable);
            updateInteractions();
        }
    }

    public void OnInterActionExecute()
    {
        if (all_interactions.Count == 0)
        {
            return;
        }
        GD.Print(all_interactions[0]?.ToString());
    }

    private void updateInteractions()
    {
        if (all_interactions.Count > 0)
        {
            InteractLabel.Text = all_interactions[0].getInteractLabel();
        }
        else
        {
            InteractLabel.Text = "";
        }
    }

    private void executeInteraction()
    {
        if (all_interactions.Count > 0)
        {
            Interaction_Area currentInteraction = all_interactions[0];

            switch (currentInteraction.getInteractionType())
            {
                case "print_text":
                    GD.Print(currentInteraction.getInteractionValue());
                    break;

                default:
                    GD.Print("Default");
                    break;
            }
        }
    }


    private void ChangeCurrentInteractionCollisionShapeDirection()
    {
        if (_CharacterParent._CurrentLookingDirection != Vector2.Zero)
        {
            _InteractionAreaCollisionShape.Position = _CharacterParent._CurrentLookingDirection * 10;
            _InteractionAreaCollisionShape.Rotation = _CharacterParent._CurrentLookingDirection.Angle();
            //_InteractionAreaCollisionShape.Position = _CharacterParent._CurrentLookingDirection.Normalized() * _InteractionOffset.Length();
        }

    }
}
