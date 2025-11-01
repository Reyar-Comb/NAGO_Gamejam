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
	private Marker2D _leftHoldMarker;
	private Marker2D _rightHoldMarker;
	private Marker2D _downHoldMarker;
	private Sprite2D _cuisineDisplaySprite;
	private Area2D _collisionArea;
	private List<string> _directionStack = new();
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_leftHoldMarker = Storage.GetNode<Marker2D>("LeftHoldMarker");
		_rightHoldMarker = Storage.GetNode<Marker2D>("RightHoldMarker");
		_downHoldMarker = Storage.GetNode<Marker2D>("DownHoldMarker");
		_stateTree = Storage.GetNode<StateTree>("StateTree");
		_cuisineDisplaySprite = Storage.GetNode<Sprite2D>("CuisineDisplaySprite");
		_collisionArea = Storage.GetNode<Area2D>("CollisionArea");
		_collisionArea.AreaEntered += OnCollisionAreaEntered;
	}
	private void OnCollisionAreaEntered(Area2D area)
	{
		if (area.IsInGroup("BananaPeel"))
		{
			AskTransit("Slip");
			area.QueueFree();
		}
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
	protected override void FrameUpdate(double delta)
	{
		if (_player.CurrentCuisine is null) return;
		switch (GetLastDirection())
		{
			case "Left":
				_cuisineDisplaySprite.GlobalPosition = _leftHoldMarker.GlobalPosition;
				_cuisineDisplaySprite.Visible = true;
				break;
			case "Right":
				_cuisineDisplaySprite.GlobalPosition = _rightHoldMarker.GlobalPosition;
				_cuisineDisplaySprite.Visible = true;
				break;
			case "Down":
				_cuisineDisplaySprite.GlobalPosition = _downHoldMarker.GlobalPosition;
				_cuisineDisplaySprite.Visible = true;
				break;
			case "Up":
				_cuisineDisplaySprite.Visible = false;
				break;
		}
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
		}
	}
	public string GetLastDirection()
	{
		if (_directionStack.Count > 0)
			return _directionStack[^1];

		return null;
	}
}
