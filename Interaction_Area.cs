using Godot;
using System;

public partial class Interaction_Area : Area2D, IInteractable
{
    [Export]
    public String InteractLabel = "none";

    [Export]
    private String InteractType = "none";

    [Export]
    private String InteractValue = "none";

    public String GetInteractionLabel()
    {
        return InteractLabel;
    }

    public void Interact(Character user)
    {
        switch (InteractType)
        {
            case "print_text":
                GD.Print(InteractValue);
                break;

            default:
                GD.Print("Default interaction on " + Name);
                break;
        }
    }

    // Kept for backward compatibility with non-interactable consumers.
    public String getInteractLabel()
    {
        return InteractLabel;
    }

    public String getInteractionType()
    {
        return InteractType;
    }

    public String getInteractionValue()
    {
        return InteractValue;
    }
}
