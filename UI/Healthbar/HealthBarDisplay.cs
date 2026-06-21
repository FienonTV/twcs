using Godot;

public partial class HealthBarDisplay : Control
{
    /****************************** EVENTS & SIGNALS ******************************/


    /****************************** EXPORT VARIABLES ******************************/


    /****************************** NODE VARIABLES ******************************/
    private ProgressBar _Hurtbar;
    private ProgressBar _Healthbar;
    private Timer _VisibleTimer;
    private Character _CharacterParent;
    private HealthComponent _HealthComponent;

    /****************************** OTHER VARIABLES ******************************/


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        this.Hide();
        //Inizialize node variables
        FindCharacterParent();

        _Hurtbar = GetNode<ProgressBar>("Hurtbar");
        _Healthbar = GetNode<ProgressBar>("Healthbar");
        _HealthComponent =
            _CharacterParent.FindChild("HealthComponent", recursive: true) as HealthComponent;
        _VisibleTimer = GetNode<Timer>("VisibleTimer");

        _Healthbar.MaxValue = _HealthComponent.GetMaxHealth();
        _Healthbar.Value = _HealthComponent.GetHealth();

        _Hurtbar.MaxValue = _HealthComponent.GetMaxHealth();
        _Hurtbar.Value = _HealthComponent.GetHealth();

        _VisibleTimer.Timeout += HideHealthBar;

        _HealthComponent._HealthChanged += OnHealthChanged;
        _HealthComponent._MaxHealthChanged += OnMaxHealthChanged;
        _HealthComponent._HealthEmpty += OnHealthEmpty;
    }

    /****************************** EVENTHANDLER ******************************/
    private void OnHealthChanged(int health)
    {
        DisplayDamage(health);
    }

    private void OnMaxHealthChanged(int maxHealth)
    {
        _Healthbar.MaxValue = maxHealth;
        _Hurtbar.MaxValue = maxHealth;
    }

    private void OnHealthEmpty()
    {
        HideHealthBar();
    }

    /****************************** OTHER METHODS ******************************/
    public void DisplayDamage(int health)
    {
        if (_Healthbar.Value != health)
        {
            this.Show();
            _Healthbar.Value = health;
            Tween tween = CreateTween();
            tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
            tween.TweenProperty(_Hurtbar, "value", _Healthbar.Value, 0.3);

            _VisibleTimer.Start();
        }
    }

    private void HideHealthBar()
    {
        this.Hide();
    }

    public void FindCharacterParent()
    {
        Node node = this;
        while (node != null) // Solange ein Parent existiert
        {
            if (node is Character character) // Prüfen, ob es vom Typ Character (oder abgeleitet) ist
            {
                _CharacterParent = character;
                GD.Print("Parent gefunden"); // Charakter gefunden, zurückgeben
            }
            node = node.GetParent(); // Zum nächsten Parent wechseln
        }
    }

    /****************************** GETTER & SETTER METHODS ******************************/
}
