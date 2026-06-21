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

    private List<IInteractable> all_interactions = new List<IInteractable>();

    public override void _Ready()
    {
        _CharacterParent = FindParent("Player") as Player;
        if (_CharacterParent == null)
        {
            GD.PrintErr("PlayerInteractionComponents: No Player parent found.");
        }

        InteractionArea = GetNodeOrNull<Interaction_Area>("InteractionArea");
        if (InteractionArea == null)
        {
            GD.PrintErr("PlayerInteractionComponents: No InteractionArea found.");
            _isLoaded = true;
            return;
        }

        _InteractionAreaCollisionShape = InteractionArea.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (_InteractionAreaCollisionShape == null)
        {
            GD.PrintErr("PlayerInteractionComponents: InteractionArea has no CollisionShape2D.");
        }

        InteractLabel = FindChild("InteractLabel", recursive: true) as Label;

        InteractionArea.AreaEntered += on_interaction_area_entered;
        InteractionArea.AreaExited += on_interaction_area_exited;

        updateInteractions();
        _isLoaded = true;
    }

    public override void _Process(double delta)
    {
        if (!_isLoaded)
        {
            return;
        }

        ChangeCurrentInteractionCollisionShapeDirection();

        if (Input.IsActionJustPressed("interact"))
        {
            executeInteraction();
        }
    }

    private void on_interaction_area_entered(Area2D area)
    {
        if (area is IInteractable interactable)
        {
            all_interactions.Insert(0, interactable);
            updateInteractions();
        }
    }

    private void on_interaction_area_exited(Area2D area)
    {
        if (area is IInteractable interactable)
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
        if (InteractLabel == null)
        {
            return;
        }
        if (all_interactions.Count > 0)
        {
            InteractLabel.Text = all_interactions[0].GetInteractionLabel();
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
            IInteractable currentInteraction = all_interactions[0];
            currentInteraction.Interact(_CharacterParent);
        }
    }

    private void ChangeCurrentInteractionCollisionShapeDirection()
    {
        if (_InteractionAreaCollisionShape == null || _CharacterParent == null)
        {
            return;
        }
        if (_CharacterParent._CurrentLookingDirection != Vector2.Zero)
        {
            _InteractionAreaCollisionShape.Position = _CharacterParent._CurrentLookingDirection * 10;
            _InteractionAreaCollisionShape.Rotation = _CharacterParent._CurrentLookingDirection.Angle();
        }
    }
}
