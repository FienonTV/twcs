/*
 * CharacterMovementComponent Class
 * --------------------------------
 * This class is responsible for controlling the movement of a character in the game.
 * It manages the movement direction, speed, and position of the character, ensuring
 * that the character stays within a specified patrol radius.
 * 
 * Key Responsibilities:
 * - Manage the movement direction and speed of the character.
 * - Update the character's position based on the movement direction.
 * - Trigger events when the movement direction changes.
 * 
 * This class is a crucial part of the movement system in the project, ensuring that
 * characters move correctly and smoothly.
 */

using System;
using Godot;

public partial class CharacterMovementComponent : Node, IMovementComponent
{
    /****************************** EVENTS & SIGNALS ******************************/
    // Event that is triggered when the character's movement direction changes
    public event Action<string, Vector2> _OnMovingPerformed;

    /****************************** EXPORT VARIABLES ******************************/
    [Export] private int _CharacterSpeed; // Speed of the character
    [Export] private float _PatrolRadius = 50F; // Radius within which the character patrols

    /****************************** NODE VARIABLES ******************************/
    public Character _Character; // Reference to the parent character node

    /****************************** OTHER VARIABLES ******************************/
    public Vector2 _StartPosition; // Start position of the character
    public Vector2 _CurrentPosition; // Current position of the character
    private Vector2 _WalkingDirection = new Vector2(1, 0); // Initial walking direction (right)
    private Vector2 _LastWalkingDirection; // Last walking direction to detect changes

    /****************************** CALLBACK METHODS ******************************/
    // This method is called when the node is added to the scene
    public override void _Ready()
    {
        _Character = GetParent<Character>(); // Get the parent character node
        if (_Character == null)
        {
            Logger.Error("CharacterMovementComponent: Parent is not a Character.");
            return;
        }
        _StartPosition = _Character.GlobalPosition; // Set the start position to the character's initial position
        if (_Character.navigationAgent2D != null)
        {
            _Character.navigationAgent2D.VelocityComputed += SafeVelocityComputed; // Connect the VelocityComputed signal to the SafeVelocityComputed method
        }
    }

    /****************************** EVENTHANDLER ******************************/
    // Event handler for velocity computation
    private void SafeVelocityComputed(Vector2 velocity)
    {
        _Character.Velocity = velocity; // Update the character's velocity
    }

    /****************************** OTHER METHODS ******************************/
    public void HandleMovement(Vector2 direction)
    {
        if (_Character == null)
        {
            Logger.Error("CharacterMovementComponent: Parent is null.");
            return;
        }

        // Check if the character has reached the left patrol boundary
        if (_CurrentPosition.X <= _StartPosition.X - _PatrolRadius)
        {
            // Change direction to right if not already moving right
            if (_WalkingDirection != Vector2.Right)
            {
                Logger.Debug("Direction Changed to Right");
                _WalkingDirection = Vector2.Right;
            }
        }
        // Check if the character has reached the right patrol boundary
        else if (_CurrentPosition.X >= _StartPosition.X + _PatrolRadius)
        {
            // Change direction to left if not already moving left
            if (_WalkingDirection != Vector2.Left)
            {
                Logger.Debug("Direction Changed to Left");
                _WalkingDirection = Vector2.Left;
            }
        }

        // Check if the walking direction has changed
        if (_WalkingDirection != _LastWalkingDirection)
        {
            // Only invoke the event if the direction has changed
            _OnMovingPerformed?.Invoke("MoveState", _WalkingDirection);
            _LastWalkingDirection = _WalkingDirection; // Update the last walking direction
        }

        // Calculate the velocity based on the walking direction and character speed
        Vector2 velocity = _WalkingDirection * _CharacterSpeed;

        // Update the character's velocity based on whether avoidance is enabled
        if (_Character.navigationAgent2D != null && _Character.navigationAgent2D.AvoidanceEnabled)
        {
            _Character.navigationAgent2D.Velocity = velocity;
        }
        else
        {
            _Character.Velocity = velocity;
            _Character.MoveAndSlide();
        }

        // Update the current position of the character
        _CurrentPosition = _Character.GlobalPosition;
    }

    /****************************** GETTER & SETTER METHODS ******************************/
    // Add any getter and setter methods here if needed
}