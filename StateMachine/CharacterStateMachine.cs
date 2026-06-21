using Godot;

public partial class CharacterStateMachine : Node2D
{
	protected CharacterState _CurrentState;
	protected CharacterState _PreviousState;
	[Export]
	protected CharacterState _DefaultState;
	public Vector2 _CurrentDirection;
	protected Player _CurrentScenePlayer;
	public AnimationPlayer _AnimationPlayer;

	protected AnimationController _AnimationController;

	public override async void _Ready()
	{
		await ToSignal(GetTree(), "process_frame");
		_CurrentState = _DefaultState;
		_PreviousState = _CurrentState;
		_AnimationPlayer = Owner.GetNode<AnimationPlayer>("AnimationPlayer");

		if (Owner is Player)
		{
			_CurrentScenePlayer = Owner as Player;
		}
		else
		{
			_CurrentScenePlayer = GetTree().GetNodesInGroup("Player")[0] as Player;
		}
		_CurrentState.Enter();
	}

	protected void ChangeState(string state)
	{
		CharacterState newState = FindChild(state) as CharacterState;

		if (newState != null)
		{
			_PreviousState.Exit();
			newState.Enter();
			_CurrentState = newState;
			_PreviousState = _CurrentState;
		}
		else
		{
			GD.PrintErr("State '" + state + "' not found");
		}
	}
}
