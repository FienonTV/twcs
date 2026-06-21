using System;
using Godot;

public partial class HealthComponent : Node
{
    /****************************** EVENTS & SIGNALS ******************************/
    public event Action<int> _MaxHealthChanged;
    public event Action<int> _HealthChanged;
    public event Action _HealthEmpty;



    /****************************** EXPORT VARIABLES ******************************/
    [Export]
    private int _MaxHealth;

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
        _Health = _MaxHealth;
        _InvulnerableTimer = GetNodeOrNull<Timer>("InvulnerableTimer");
        if (_InvulnerableTimer != null)
        {
            _InvulnerableTimer.Timeout += StopTemporaryInvulnerability;
        }
        else
        {
            GD.PrintErr("HealthComponent: InvulnerableTimer not found.");
        }
    }

    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/

    //Increases or decreases MaxHealth depended on the passed Variable (+/-)
    public void ChangeMaxHealth(int change)
    {
        _MaxHealth += change;
        _MaxHealthChanged?.Invoke(_MaxHealth);
    }

    public void ChangeCurrentHealth(int change)
    {
        GD.Print("Changed Current Health");
        _Health += change;
        ClampHealth();
        _HealthChanged?.Invoke(_Health);
        GD.Print("Current Health is: " + _Health);
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
        _HealthChanged?.Invoke(_Health);

        //Currently Setting the Health to maxHealt if Health is 0 (Dead)
        if (_Health <= 0)
        {
            _HealthEmpty?.Invoke();
        }

        GD.Print("Current Health: " + _Health);
    }

    private void ClampHealth()
    {
        _Health = Mathf.Clamp(_Health, 0, _MaxHealth);
    }

    //Makes the Parent Invulnerable for specific time
    public void StartTemporaryInvulnerability()
    {
        GD.Print("Invulnerable");
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
        GD.Print("Not Invulnerable anymore");
        _Invulnerable = false;
    }

    /****************************** GETTER & SETTER METHODS ******************************/
    public int GetMaxHealth()
    {
        return _MaxHealth;
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
