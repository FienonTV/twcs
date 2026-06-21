using Godot;

public partial class Chest : StaticBody2D, IInteractable
{
    [Export]
    public string InteractionLabel = "Open Chest";

    public override void _Ready()
    {
        // Initialization can go here, e.g. populate inventory.
    }

    public string GetInteractionLabel()
    {
        return InteractionLabel;
    }

    public void Interact(Character user)
    {
        if (user == null)
        {
            Logger.Error("Chest: No user provided.");
            return;
        }
        Logger.Debug($"Chest opened by {user.Name}");
        // TODO: open chest UI / give loot
    }

    // Legacy signature kept for backward compatibility with any direct callers.
    public void Interact()
    {
        Logger.Debug("Chest opened.");
    }
}
