/*
 * AnimationController Class
 * -------------------------
 * Manages character and effect animations.
 */

using Godot;

public partial class AnimationController : Node
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AnimationPlayer EffectPlayer;

    [Export]
    private Timer HurtEffectTimer;

    public override void _Ready()
    {
        base._Ready();

        if (AnimationPlayer == null)
        {
            AnimationPlayer = FindChild("AnimationPlayer") as AnimationPlayer;
        }
        if (EffectPlayer == null)
        {
            EffectPlayer = GetNodeOrNull<AnimationPlayer>("EffectPlayer");
        }
        if (HurtEffectTimer == null)
        {
            HurtEffectTimer = FindChild("HurtEffectTimer") as Timer;
        }

        if (AnimationPlayer == null)
        {
            Logger.Debug("AnimationController: AnimationPlayer == null");
        }
        if (HurtEffectTimer == null)
        {
            Logger.Warning("AnimationController: HurtEffectTimer not assigned.");
        }
        else
        {
            HurtEffectTimer.Timeout += StopEffect;
        }
    }

    public void PlayAnimation(string animationName)
    {
        Logger.Debug($"AnimationController: Playing: {animationName}");
        if (AnimationPlayer != null && AnimationPlayer.HasAnimation(animationName))
        {
            AnimationPlayer.Play(animationName);
        }
    }

    public void StopAnimation()
    {
        AnimationPlayer?.Stop();
    }

    public bool IsAnimationPlaying()
    {
        return AnimationPlayer != null && AnimationPlayer.IsPlaying();
    }

    public void PlayEffect(string effectName)
    {
        EffectPlayer?.Play(effectName);
        HurtEffectTimer?.Start();
    }

    private void StopEffect()
    {
        EffectPlayer?.Play("RESET");
    }
}
