using Godot;

public partial class newStateMachine : Node2D
{
	protected newState _CurrentState;
	protected newState _PreviousState;
	[Export]
	protected newState _DefaultState;
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
		_CurrentState = FindChild(state) as newState;

		if (_CurrentState != null)
		{
			_CurrentState.Enter();
			_PreviousState.Exit();
			_PreviousState = _CurrentState;
		}
		else
		{
			GD.PrintErr("State '" + state + "' not found");
		}
	}
}
