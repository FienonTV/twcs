using System;
using Godot;


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
    public Sword _Tool;
    public Character _CharacterParent;
    public CollisionShape2D _CollisionShape2D;


    /****************************** OTHER VARIABLES ******************************/
    public bool _IsActive = false;


    /****************************** CALLBACK METHODS ******************************/
    public override void _Ready()
    {
        base._Ready();
        _CollisionShape2D = FindChild("CollisionShape2D", recursive: true) as CollisionShape2D;
        FindSwordParent();
        FindCharacterParent();
    }

    public override void _Process(double delta)
    {
        ChangeCurrentHitboxPosition();
    }


    /****************************** EVENTHANDLER ******************************/


    /****************************** OTHER METHODS ******************************/
    public void ActivateHitBox()
    {
        GD.Print("HitBox activated and performing Hit");
        _IsActive = true;
        _OnHitboxActivated?.Invoke(_Tool._Damage);
    }

    public void DeactivateHitBox()
    {
        GD.Print("Hitbox Deactivated");
        _IsActive = false;
    }

    public void FindSwordParent()
    {
        Node node = this;
        while (node != null) // Solange ein Parent existiert
        {
            if (node is Sword sword) // Prüfen, ob es vom Typ Character (oder abgeleitet) ist
            {
                _Tool = sword;
                GD.Print("Parent gefunden"); // Charakter gefunden, zurückgeben
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
            }
            node = node.GetParent(); // Zum nächsten Parent wechseln
        }
    }

    private void ChangeCurrentHitboxPosition()
    {
        Position = _CharacterParent._CurrentLookingDirection * 20;
        Rotation = _CharacterParent._CurrentLookingDirection.Angle();

    }

    /****************************** GETTER & SETTER METHODS ******************************/
}
