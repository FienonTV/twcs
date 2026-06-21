using Godot;
using System.Collections.Generic;

public partial class PlayerInteractionComponents : Node2D
{
    private Character _OwnerCharacter;

    [Export]
    private Interaction_Area InteractionArea;

    [Export]
    private CollisionShape2D InteractionAreaCollisionShape;

    [Export]
    private Label InteractLabel;

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

        if (InteractionArea == null)
        {
            InteractionArea = FindChild("InteractionArea") as Interaction_Area;
        }
        if (InteractionArea == null)
        {
            Logger.Error("PlayerInteractionComponents: No InteractionArea found.");
            _isLoaded = true;
            return;
        }

        if (InteractionAreaCollisionShape == null)
        {
            InteractionAreaCollisionShape = InteractionArea.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        }
        if (InteractionAreaCollisionShape == null)
        {
            Logger.Error("PlayerInteractionComponents: InteractionArea has no CollisionShape2D.");
        }

        if (InteractLabel == null)
        {
            InteractLabel = FindChild("InteractLabel", recursive: true) as Label;
        }

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
        if (InteractLabel == null)
        {
            return;
        }
        if (_AllInteractions.Count > 0)
        {
            InteractLabel.Text = _AllInteractions[0].GetInteractionLabel();
        }
        else
        {
            InteractLabel.Text = "";
        }
    }

    private void ExecuteInteraction()
    {
        if (_AllInteractions.Count > 0)
        {
            IInteractable currentInteraction = _AllInteractions[0];
            currentInteraction.Interact(_OwnerCharacter);
        }
    }

    private void ChangeCurrentInteractionCollisionShapeDirection()
    {
        if (InteractionAreaCollisionShape == null || _OwnerCharacter == null)
        {
            return;
        }
        if (_OwnerCharacter.CurrentLookingDirection != Vector2.Zero)
        {
            InteractionAreaCollisionShape.Position = _OwnerCharacter.CurrentLookingDirection * 10;
            InteractionAreaCollisionShape.Rotation = _OwnerCharacter.CurrentLookingDirection.Angle();
        }
    }

    private bool FindCharacterParent()
    {
        Node node = this;
        while (node != null)
        {
            if (node is Character character)
            {
                _OwnerCharacter = character;
                return true;
            }
            node = node.GetParent();
        }
        return false;
    }
}
