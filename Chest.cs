using Godot;

public partial class Chest : StaticBody2D, IInteractable
{
    [Export]
    public string _InteractionLabel = "Open Chest";

    public override void _Ready()
    {
        // Initialization can go here, e.g. populate inventory.
    }

    public string GetInteractionLabel()
    {
        return _InteractionLabel;
    }

    public void Interact(Character user)
    {
        Logger.Debug("Chest opened by " + user.Name);
        // TODO: open chest UI / give loot
    }

    // Legacy signature kept for backward compatibility with any direct callers.
    public void Interact()
    {
        Logger.Debug("Chest opened.");
    }
}
