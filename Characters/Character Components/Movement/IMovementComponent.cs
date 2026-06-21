using Godot;

/****************************** MOVEMENT COMPONENT INTERFACE ******************************/
/// <summary>
/// Common interface for all character movement components.
/// Implementations handle velocity/position updates for specific actor types
/// (Player, NPC, flying enemies, etc.).
/// </summary>
public interface IMovementComponent
{
    void HandleMovement(Vector2 direction);
}
