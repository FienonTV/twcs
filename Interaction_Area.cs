using Godot;
using System;

public partial class Interaction_Area : Area2D
{

    [Export] public String InteractLabel = "none";

    [Export] private String InteractType = "none";

    [Export] private String InteractValue = "none";


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
