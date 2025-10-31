using Godot;
using System;

public partial class PlayerMoveControl : State
{
	private Player _player;
	private float _speed;
	private float _finalSpeed;
	private StateTree _stateTree;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_speed = Storage.GetVariant<float>("Speed");
		_stateTree = Storage.GetNode<StateTree>("StateTree");
	}
	protected override void PhysicsUpdate(double delta)
	{
		Vector2 velocity = _player.Velocity;

		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		
		if (_stateTree.CurrentState.Name == "Dash")
		{
			_finalSpeed = _speed * 2;
		}
		else
		{
			_finalSpeed = _speed;
		}

		if (direction != Vector2.Zero)
			velocity = direction * _finalSpeed;
		else
			velocity = velocity.MoveToward(Vector2.Zero, _speed);

		_player.Velocity = velocity;
		_player.MoveAndSlide();
	}
}
