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
		AudioManager.Instance.LoadSFX("FootNorm", "res://Assets/SoundFX/FootNorm2.mp3");
	}

	protected override void Enter()
	{
		GD.Print("Player has entered Walk State.");
		Storage.SetVariant("AnimationSpeedMultiplier", 1.0f);
		AudioManager.Instance.PlaySFX("FootNorm");
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

	protected override void Exit()
	{
		AudioManager.Instance.StopSFX("FootNorm");
	}
}
