using Godot;

public partial class HealthBarDisplay : Control
{
    [Export]
    private ProgressBar Hurtbar;

    [Export]
    private ProgressBar Healthbar;

    [Export]
    private Timer VisibleTimer;

    private Character _OwnerCharacter;

    [Export]
    private HealthComponent HealthComponent;

    public override void _Ready()
    {
        this.Hide();

        if (!FindCharacterParent())
        {
            Logger.Error("HealthBarDisplay: No Character parent found. Disabling health bar.");
            return;
        }

        if (Hurtbar == null || Healthbar == null || VisibleTimer == null)
        {
            Logger.Error("HealthBarDisplay: Required UI nodes missing.");
            return;
        }

        if (HealthComponent == null)
        {
            HealthComponent = _OwnerCharacter.FindChild("HealthComponent", recursive: true) as HealthComponent;
        }
        if (HealthComponent == null)
        {
            Logger.Error($"HealthBarDisplay: No HealthComponent found on '{_OwnerCharacter.Name}'.");
            return;
        }

        Healthbar.MaxValue = HealthComponent.GetMaxHealth();
        Healthbar.Value = HealthComponent.GetHealth();

        Hurtbar.MaxValue = HealthComponent.GetMaxHealth();
        Hurtbar.Value = HealthComponent.GetHealth();

        VisibleTimer.Timeout += HideHealthBar;

        HealthComponent.HealthChanged += OnHealthChanged;
        HealthComponent.MaxHealthChanged += OnMaxHealthChanged;
        HealthComponent.HealthEmpty += OnHealthEmpty;
    }

    private void OnHealthChanged(int health)
    {
        DisplayDamage(health);
    }

    private void OnMaxHealthChanged(int maxHealth)
    {
        Healthbar.MaxValue = maxHealth;
        Hurtbar.MaxValue = maxHealth;
    }

    private void OnHealthEmpty()
    {
        HideHealthBar();
    }

    public void DisplayDamage(int health)
    {
        if (Healthbar == null || Hurtbar == null || VisibleTimer == null)
        {
            return;
        }

        if (Healthbar.Value != health)
        {
            this.Show();
            Healthbar.Value = health;
            Tween tween = CreateTween();
            tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
            tween.TweenProperty(Hurtbar, "value", Healthbar.Value, 0.3);

            VisibleTimer.Start();
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
                _OwnerCharacter = character;
                Logger.Debug($"HealthBarDisplay: Character parent '{character.Name}' found.");
                return true;
            }
            node = node.GetParent();
        }
        return false;
    }
}
