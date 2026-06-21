using System;
using Godot;

public partial class HealthComponent : Node
{
    /****************************** EVENTS & SIGNALS ******************************/
    public event Action<int> MaxHealthChanged;
    public event Action<int> HealthChanged;
    public event Action HealthEmpty;

    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    private int MaxHealth;

    [Export]
    private bool Invulnerable = false;

    [Export]
    private float InvulnerableDuration = 1.5f;

    [Export]
    private int Health = 1;

    /****************************** NODE VARIABLES ******************************/
    [Export]
    private Timer InvulnerableTimer;

    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        Health = MaxHealth;

        if (InvulnerableTimer == null)
        {
            InvulnerableTimer = GetNodeOrNull<Timer>("InvulnerableTimer");
        }
        if (InvulnerableTimer != null)
        {
            InvulnerableTimer.Timeout += StopTemporaryInvulnerability;
        }
        else
        {
            Logger.Warning("HealthComponent: InvulnerableTimer not assigned.");
        }
    }

    /****************************** OTHER METHODS ******************************/

    public void ChangeMaxHealth(int change)
    {
        MaxHealth += change;
        MaxHealthChanged?.Invoke(MaxHealth);
    }

    public void ChangeCurrentHealth(int change)
    {
        Logger.Debug("Changed Current Health");
        Health += change;
        ClampHealth();
        HealthChanged?.Invoke(Health);
        Logger.Debug($"Current Health is: {Health}");
    }

    public void ChangeHealth(int change)
    {
        if (change < 0)
        {
            if (Invulnerable)
            {
                return;
            }
            else
            {
                Health += change;
            }
            StartTemporaryInvulnerability();
        }
        else
        {
            Health += change;
        }

        ClampHealth();
        HealthChanged?.Invoke(Health);

        if (Health <= 0)
        {
            HealthEmpty?.Invoke();
        }

        Logger.Debug($"Current Health: {Health}");
    }

    private void ClampHealth()
    {
        Health = Mathf.Clamp(Health, 0, MaxHealth);
    }

    public void StartTemporaryInvulnerability()
    {
        Logger.Debug("Invulnerable");
        if (InvulnerableTimer != null)
        {
            InvulnerableTimer.WaitTime = InvulnerableDuration;
            Invulnerable = true;
            InvulnerableTimer.Start();
        }
    }

    public void StopTemporaryInvulnerability()
    {
        Logger.Debug("Not Invulnerable anymore");
        Invulnerable = false;
    }

    /****************************** GETTER & SETTER METHODS ******************************/
    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public int GetHealth()
    {
        return Health;
    }

    public void SetHealth(int health)
    {
        Health = health;
    }
}
