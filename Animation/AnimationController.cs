/*
 * AnimationController Class
 * -------------------------
 * This class is responsible for managing and playing animations for characters and effects in the game.
 * It interacts with AnimationPlayer nodes to control animations based on game events and states.
 * 
 * Key Responsibilities:
 * - Play and stop character animations.
 * - Handle effect animations.
 * - Manage animation-related timers and signals.
 * 
 * This class is a crucial part of the animation system in the project, ensuring that characters and effects
 * are animated correctly based on the game's logic and state transitions.
 */

using Godot;

public partial class AnimationController : Node
{
    // Reference to the AnimationPlayer node that handles character animations
    public AnimationPlayer _AnimationPlayer;

    // Reference to the AnimationPlayer node that handles effect animations
    public AnimationPlayer _EffectPlayer;

    // Timer used for handling hurt effect duration
    private Timer _HurtEffectTimer;

    // This method is called when the node is added to the scene
    public override void _Ready()
    {
        // Find and store the reference to the AnimationPlayer node
        _AnimationPlayer = FindChild("AnimationPlayer") as AnimationPlayer;

        // Get the reference to the EffectPlayer node
        _EffectPlayer = GetNodeOrNull<AnimationPlayer>("EffectPlayer");

        // Find and store the reference to the HurtEffectTimer node
        _HurtEffectTimer = FindChild("HurtEffectTimer") as Timer;

        // Check if the AnimationPlayer node was found
        if (_AnimationPlayer == null)
        {
            GD.Print("AnimationPlayer == null");
        }

        // Connect the Timeout signal of the HurtEffectTimer to the StopEffect method
        _HurtEffectTimer.Timeout += StopEffect;
    }

    // Method to play a specified animation
    public void PlayAnimation(string animationName)
    {
        GD.Print("Playing: " + animationName);
        // Check if the AnimationPlayer has the specified animation
        if (_AnimationPlayer.HasAnimation(animationName))
        {
            // Play the specified animation
            _AnimationPlayer?.Play(animationName);
        }
    }

    // Method to stop the currently playing animation
    public void StopAnimation()
    {
        _AnimationPlayer?.Stop();
    }

    // Method to check if any animation is currently playing
    public bool IsAnimationPlaying()
    {
        return _AnimationPlayer != null && _AnimationPlayer.IsPlaying();
    }

    // Method to play a specified effect animation
    public void PlayEffect(string effectName)
    {
        _EffectPlayer?.Play(effectName);
        _HurtEffectTimer.Start();
    }

    // Method to stop the effect animation
    private void StopEffect()
    {
        _EffectPlayer?.Play("RESET");
    }
}