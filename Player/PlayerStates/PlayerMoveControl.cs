using Godot;
using System;
using System.Collections.Generic;
public partial class PlayerMoveControl : State
{
	private bool HeadingRight
	{
		get => Storage.GetVariant<bool>("HeadingRight");
		set => Storage.SetVariant("HeadingRight", value);
	}
	private bool HeadingUp
	{
		get => Storage.GetVariant<bool>("HeadingUp");
		set => Storage.SetVariant("HeadingUp", value);
	}
	private float Speed => Storage.GetVariant<float>("Speed");
	private float SpeedMultiplier => Storage.GetVariant<float>("SpeedMultiplier");
	private float FinalSpeed =>
		_stateTree.CurrentState.Name == "Dash"
		? Speed * SpeedMultiplier
		: Speed;
	private Player _player;
	private AnimatedSprite2D _sprite;
	private StateTree _stateTree;
	private List<string> _directionStack = new();
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_stateTree = Storage.GetNode<StateTree>("StateTree");
	}
	protected override void PhysicsUpdate(double delta)
	{
		Vector2 velocity = _player.Velocity;

		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		UpdateDirectionStack();
		if (direction != Vector2.Zero)
		{
			velocity = direction * FinalSpeed;
			SetHeadings(direction);
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, Speed);
		}

		_player.Velocity = velocity;
		_player.MoveAndSlide();
	}
	private void SetHeadings(Vector2 direction)
	{
		HeadingRight = direction.X >= 0;
		HeadingUp = direction.Y <= 0;
		if (Mathf.IsZeroApprox(direction.Y) || GetLastDirection() is "Left" or "Right")
			_sprite.FlipH = HeadingRight;
		else
			_sprite.FlipH = false;
	}
	private void UpdateDirectionStack()
	{
		UpdateAction("Up");
		UpdateAction("Down");
		UpdateAction("Left");
		UpdateAction("Right");
	}
	private void UpdateAction(string action)
	{
		if (Input.IsActionJustPressed(action))
		{
			if (!_directionStack.Contains(action))
				_directionStack.Add(action);
		}
		else if (Input.IsActionJustReleased(action))
		{
			_directionStack.Remove(action);
			Storage.SetVariant("LastRemovedDirection", action);
			GD.Print($"Last Removed Direction: {action}");
		}
	}
	public string GetLastDirection()
	{
		if (_directionStack.Count > 0)
			return _directionStack[^1];

		return null;
	}
}
