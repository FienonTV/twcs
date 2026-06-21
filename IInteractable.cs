/// <summary>
/// Defines an object that the player (or any Character) can interact with.
/// Implement this on Area2D-based nodes to provide a label and an interaction effect.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Returns the label shown in the interaction prompt UI.
    /// </summary>
    string GetInteractionLabel();

    /// <summary>
    /// Called when the user activates the interaction.
    /// </summary>
    /// <param name="user">The Character that interacted with this object.</param>
    void Interact(Character user);
}
