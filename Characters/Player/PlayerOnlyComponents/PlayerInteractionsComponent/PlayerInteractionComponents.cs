using Godot;
using System.Collections.Generic;

public partial class PlayerInteractionComponents : Node2D
{
    private Character OwnerCharacter;
    private Interaction_Area InteractionArea;
    private CollisionShape2D _InteractionAreaCollisionShape;

    private Label _InteractLabel;

    private bool _isLoaded = false;

    private List<IInteractable> _AllInteractions = new List<IInteractable>();

    public override void _Ready()
    {
        if (!FindCharacterParent())
        {
            Logger.Error("PlayerInteractionComponents: No Character parent found.");
            _isLoaded = true;
            return;
        }

        InteractionArea = GetNodeOrNull<Interaction_Area>("InteractionArea");
        if (InteractionArea == null)
        {
            Logger.Error("PlayerInteractionComponents: No InteractionArea found.");
            _isLoaded = true;
            return;
        }

        _InteractionAreaCollisionShape = InteractionArea.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (_InteractionAreaCollisionShape == null)
        {
            Logger.Error("PlayerInteractionComponents: InteractionArea has no CollisionShape2D.");
        }

        _InteractLabel = FindChild("InteractLabel", recursive: true) as Label;

        InteractionArea.AreaEntered += OnInteractionAreaEntered;
        InteractionArea.AreaExited += OnInteractionAreaExited;

        UpdateInteractions();
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
            ExecuteInteraction();
        }
    }

    private void OnInteractionAreaEntered(Area2D area)
    {
        if (area is IInteractable interactable)
        {
            _AllInteractions.Insert(0, interactable);
            UpdateInteractions();
        }
    }

    private void OnInteractionAreaExited(Area2D area)
    {
        if (area is IInteractable interactable)
        {
            _AllInteractions.Remove(interactable);
            UpdateInteractions();
        }
    }

    public void OnInterActionExecute()
    {
        if (_AllInteractions.Count == 0)
        {
            return;
        }
        Logger.Debug($"PlayerInteractionComponents: interacting with {_AllInteractions[0]?.GetInteractionLabel()}.");
    }

    private void UpdateInteractions()
    {
        if (_InteractLabel == null)
        {
            return;
        }
        if (_AllInteractions.Count > 0)
        {
            _InteractLabel.Text = _AllInteractions[0].GetInteractionLabel();
        }
        else
        {
            _InteractLabel.Text = "";
        }
    }

    private void ExecuteInteraction()
    {
        if (_AllInteractions.Count > 0)
        {
            IInteractable currentInteraction = _AllInteractions[0];
            currentInteraction.Interact(OwnerCharacter);
        }
    }

    private void ChangeCurrentInteractionCollisionShapeDirection()
    {
        if (_InteractionAreaCollisionShape == null || OwnerCharacter == null)
        {
            return;
        }
        if (OwnerCharacter.CurrentLookingDirection != Vector2.Zero)
        {
            _InteractionAreaCollisionShape.Position = OwnerCharacter.CurrentLookingDirection * 10;
            _InteractionAreaCollisionShape.Rotation = OwnerCharacter.CurrentLookingDirection.Angle();
        }
    }

    private bool FindCharacterParent()
    {
        Node node = this;
        while (node != null)
        {
            if (node is Character character)
            {
                OwnerCharacter = character;
                return true;
            }
            node = node.GetParent();
        }
        return false;
    }
}
