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
    private bool _Invulnerable = false;

    [Export]
    private float _InvulnerableDuration = 1.5f;

    [Export]
    private int _Health = 1;

    /****************************** NODE VARIABLES ******************************/
    private Timer _InvulnerableTimer;

    /****************************** OTHER VARIABLES ******************************/


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        _Health = MaxHealth;
        _InvulnerableTimer = GetNodeOrNull<Timer>("InvulnerableTimer");
        if (_InvulnerableTimer != null)
        {
            _InvulnerableTimer.Timeout += StopTemporaryInvulnerability;
        }
        else
        {
            Logger.Error("HealthComponent: InvulnerableTimer not found.");
        }
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/

    //Increases or decreases MaxHealth depended on the passed Variable (+/-)
    public void ChangeMaxHealth(int change)
    {
        MaxHealth += change;
        MaxHealthChanged?.Invoke(MaxHealth);
    }

    public void ChangeCurrentHealth(int change)
    {
        Logger.Debug("Changed Current Health");
        _Health += change;
        ClampHealth();
        HealthChanged?.Invoke(_Health);
        Logger.Debug("Current Health is: " + _Health);
    }

    //Increases or decreases Health depended on the passed Variable (+/-)
    public void ChangeHealth(int change)
    {
        if (change < 0)
        {
            if (_Invulnerable)
            {
                return;
            }
            else
            {
                _Health += change;
            }
            StartTemporaryInvulnerability();
        }
        else
        {
            _Health += change;
        }

        ClampHealth();
        HealthChanged?.Invoke(_Health);

        //Currently Setting the Health to maxHealt if Health is 0 (Dead)
        if (_Health <= 0)
        {
            HealthEmpty?.Invoke();
        }

        Logger.Debug("Current Health: " + _Health);
    }

    private void ClampHealth()
    {
        _Health = Mathf.Clamp(_Health, 0, MaxHealth);
    }

    //Makes the Parent Invulnerable for specific time
    public void StartTemporaryInvulnerability()
    {
        Logger.Debug("Invulnerable");
        if (_InvulnerableTimer != null)
        {
            _InvulnerableTimer.WaitTime = _InvulnerableDuration;
            _Invulnerable = true;
            _InvulnerableTimer.Start();
        }
    }

    //Stops the Invulnerability
    public void StopTemporaryInvulnerability()
    {
        Logger.Debug("Not Invulnerable anymore");
        _Invulnerable = false;
    }

    /****************************** GETTER & SETTER METHODS ******************************/
    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public int GetHealth()
    {
        return _Health;

    }
    public void SetHealth(int health)
    {
        _Health = health;
    }
}
