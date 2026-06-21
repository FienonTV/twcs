using Godot;

/// <summary>
/// FollowState is a specialization of WalkState.
/// It delegates movement direction decisions to its configured _MovementBehavior,
/// which is typically a FollowPlayerBehavior that steers the character toward the player.
/// This state contains no extra logic because the behavior-driven approach keeps the
/// state machine generic while allowing per-character movement personalities.
/// </summary>
public partial class FollowState : WalkState
{
}
