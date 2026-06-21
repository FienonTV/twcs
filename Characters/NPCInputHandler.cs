using System;
using Godot;

public partial class NPCInputHandler : Node
{/*
    [Export]
    private BaseMovementBehavior _MovementBehavior; //Movement Behavior of the Character, for possible individual behave on individual Characters
    [Export]
    private BaseAttackExecution _AttackExecution; //Attacking Behavior of the Character
    [Export]
    private BaseAttackCondition _AttackCondition;   //Conditions for Attacking
    public event Action _OnAllReady;

    [Signal]
    public delegate void _OnMoveRequestEventHandler(Vector2 direction);
    public event Action _OnAttackRequest;

    private Character _Parent; //The Character Parent of the InputHandler

    public override void _Ready()
    {
        base._Ready();
        _Parent = GetParent<Character>(); //Gets the Parent of the InputHandler, the Inputhandler need to be a direct child of the Character

    }

    public override void _Process(double delta)
    {
        GD.Print("Process");

        if (_Parent != null)
        {
            UpdateInput();

        }

    }
/*

    /** 
    ** Updates the "Input" of the Character
    ** If the Condition for attacking == true, the Attack will be executed, otherwise the MovementBehavior will be called
    *TODO: Change the Vector2.Zero to the actual target position
    **//*
    public void UpdateInput()
    {
        GD.Print("UpdatingInput");
        if (_AttackCondition.ShouldAttack())// Checking if the attacking conditions are met
        {
            _AttackExecution.ExecuteAttack(_Parent.GlobalPosition, Vector2.Zero); //We need to check here, if there is a target in the Attack Range
        }
        else
        {
            //Get the next direction dependend and calculated by the MovementBehavior
            Vector2 targetPoint = _MovementBehavior.GetNextDirection();
            EmitSignal(nameof(_OnMoveRequest), targetPoint);
        }
    }*/
}
