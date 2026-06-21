using Godot;

public partial class HealthBarDisplay : Control
{
    private ProgressBar _Hurtbar;
    private ProgressBar _Healthbar;
    private Timer _VisibleTimer;
    private Character OwnerCharacter;
    private HealthComponent _HealthComponent;

    public override void _Ready()
    {
        this.Hide();

        if (!FindCharacterParent())
        {
            Logger.Error("HealthBarDisplay: No Character parent found. Disabling health bar.");
            return;
        }

        _Hurtbar = GetNodeOrNull<ProgressBar>("Hurtbar");
        _Healthbar = GetNodeOrNull<ProgressBar>("Healthbar");
        _HealthComponent = OwnerCharacter.FindChild("HealthComponent", recursive: true) as HealthComponent;
        _VisibleTimer = GetNodeOrNull<Timer>("VisibleTimer");

        if (_Hurtbar == null || _Healthbar == null || _VisibleTimer == null)
        {
            Logger.Error("HealthBarDisplay: Required UI nodes missing.");
            return;
        }

        if (_HealthComponent == null)
        {
            Logger.Error($"HealthBarDisplay: No HealthComponent found on '{OwnerCharacter.Name}'.");
            return;
        }

        _Healthbar.MaxValue = _HealthComponent.GetMaxHealth();
        _Healthbar.Value = _HealthComponent.GetHealth();

        _Hurtbar.MaxValue = _HealthComponent.GetMaxHealth();
        _Hurtbar.Value = _HealthComponent.GetHealth();

        _VisibleTimer.Timeout += HideHealthBar;

        _HealthComponent.HealthChanged += OnHealthChanged;
        _HealthComponent.MaxHealthChanged += OnMaxHealthChanged;
        _HealthComponent.HealthEmpty += OnHealthEmpty;
    }

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

    public void DisplayDamage(int health)
    {
        if (_Healthbar == null || _Hurtbar == null || _VisibleTimer == null)
        {
            return;
        }

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

    public bool FindCharacterParent()
    {
        Node node = this;
        while (node != null)
        {
            if (node is Character character)
            {
                OwnerCharacter = character;
                Logger.Debug($"HealthBarDisplay: Character parent '{character.Name}' found.");
                return true;
            }
            node = node.GetParent();
        }
        return false;
    }
}
