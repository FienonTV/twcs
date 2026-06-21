using Godot;
using System;

public partial class Interaction_Area : Area2D, IInteractable
{
    [Export]
    public string InteractLabel = "none";

    [Export]
    private string InteractType = "none";

    [Export]
    private string InteractValue = "none";

    public string GetInteractionLabel()
    {
        return InteractLabel;
    }

    public void Interact(Character user)
    {
        switch (InteractType)
        {
            case "print_text":
                Logger.Debug(InteractValue);
                break;

            default:
                Logger.Debug($"Default interaction on {Name}");
                break;
        }
    }

    // Kept for backward compatibility with non-interactable consumers.
    public string getInteractLabel()
    {
        return InteractLabel;
    }

    public string getInteractionType()
    {
        return InteractType;
    }

    public string getInteractionValue()
    {
        return InteractValue;
    }
}
