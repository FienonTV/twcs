using Godot;
using System;


/// <summary>
/// Dependencies (What must be present for the class to work):
/// - Tool 
/// - Character that carries the Tool (inkl. AttackComponent)
/// 
/// Describes and defines the hitbox of a tool. If a tool has a HitboxComponent, it can inflict damage.
/// Reacts to signals from the attack component and activates/deactivates itself when these signals are received. 
/// Sends a signal itself when the hitbox is activated.
/// </summary>


public partial class HitBoxComponent : Area2D
{
    /****************************** EVENTS & SIGNALS ******************************/
    public event Action<int> _OnHitboxActivated;




    /****************************** EXPORT VARIABLES ******************************/


    /****************************** NODE VARIABLES ******************************/
    public HandItem _Tool;
    public Character _CharacterParent;
    public CollisionShape2D _CollisionShape2D;


    /****************************** OTHER VARIABLES ******************************/
    public bool _IsActive = false;


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        _CollisionShape2D = FindChild("CollisionShape2D", recursive: true) as CollisionShape2D;
        FindHandItemParent();
        FindCharacterParent();
    }

    public override void _Process(double delta)
    {
        if (_CharacterParent == null || _CollisionShape2D == null)
        {
            return;
        }
        ChangeCurrentHitboxPosition();
    }


    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/
    public void ActivateHitBox()
    {
        GD.Print("HitBox activated and performing Hit");
        _IsActive = true;
        _CollisionShape2D.Disabled = false;
        _OnHitboxActivated?.Invoke(_Tool?._Damage ?? 0);
    }

    public void DeactivateHitBox()
    {
        GD.Print("Hitbox Deactivated");
        _IsActive = false;
        if (_CollisionShape2D != null)
        {
            _CollisionShape2D.Disabled = true;
        }
    }

    public void FindHandItemParent()
    {
        Node node = this;
        while (node != null) // Solange ein Parent existiert
        {
            if (node is HandItem handItem) // Prüfen, ob es vom Typ Character (oder abgeleitet) ist
            {
                _Tool = handItem;
                GD.Print("Parent gefunden"); // Charakter gefunden, zurückgeben
                return;
            }
            node = node.GetParent(); // Zum nächsten Parent wechseln
        }
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
                return;
            }
            node = node.GetParent(); // Zum nächsten Parent wechseln
        }
    }

    private void ChangeCurrentHitboxPosition()
    {
        if (_CharacterParent == null)
        {
            return;
        }
        Position = _CharacterParent._CurrentLookingDirection * 20;
        Rotation = _CharacterParent._CurrentLookingDirection.Angle();

    }

    /****************************** GETTER & SETTER METHODS ******************************/
}
