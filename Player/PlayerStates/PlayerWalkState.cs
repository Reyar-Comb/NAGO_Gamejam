using Godot;
using System;

public partial class PlayerWalkState : State
{
	private float AnimationSpeedMultiplier => Storage.GetVariant<float>("AnimationSpeedMultiplier");
	private Player _player;
	private AnimatedSprite2D _sprite;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
	}

	protected override void Enter()
	{
		GD.Print("Player has entered Walk State.");
		Storage.SetVariant("AnimationSpeedMultiplier", 1.0f);
	}
	protected override void FrameUpdate(double delta)
	{
		if (Input.IsActionJustPressed("Dash"))
		{
			AskTransit("Dash");
		}
		if (_player.Velocity.IsEqualApprox(Vector2.Zero))
		{
			AskTransit("Idle");
		}
	}
}
