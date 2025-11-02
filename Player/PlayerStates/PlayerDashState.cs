using Godot;
using System;

public partial class PlayerDashState : State
{
	[Export] public PackedScene TrailScene;
	[Export] public Timer DashTrailTimer;
	[Export] public float DashTrailInterval = 0.1f;
	[Export] public float DashTrailLifetime = 0.5f;
	private Player _player;
	private AnimatedSprite2D _sprite;
	private StaminaBar _staminaBar;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_staminaBar = Storage.GetNode<StaminaBar>("StaminaBar");
		DashTrailTimer.Timeout += OnDashTrailTimerTimeout;
		AudioManager.Instance.LoadSFX("FootFast", "res://Assets/SoundFX/FootFast2.mp3");
	}

	protected override void Enter()
	{
		GD.Print("Player has entered Dash State.");
		Storage.SetVariant("AnimationSpeedMultiplier", Storage.GetVariant<float>("DashAnimationSpeedMultiplier"));
		if (!Mathf.IsEqualApprox(Storage.GetVariant<float>("SpeedMultiplier"), 1f))
		{
			DashTrailTimer.Start(DashTrailInterval);
			AudioManager.Instance.PlaySFX("FootFast");
		}
		else
		{
			DashTrailTimer.Stop();
		}
	}
	protected override void Exit()
	{
		DashTrailTimer.Stop();
		AudioManager.Instance.StopSFX("FootFast");
	}
	protected override void FrameUpdate(double delta)
	{
		if (Input.IsActionJustReleased("Dash") || _staminaBar.CurrentStamina <= 0)
		{
			AskTransit("Walk");
		}
	}
	private void OnDashTrailTimerTimeout()
	{
		Trail trail = TrailScene.Instantiate<Trail>();
		trail.GlobalPosition = _player.GlobalPosition;
		trail.RotationDegrees = _player.RotationDegrees;
		trail.Texture = _sprite.SpriteFrames.GetFrameTexture(_sprite.Animation, _sprite.Frame);
		trail.Lifetime = DashTrailLifetime;
		trail.Scale = _sprite.FlipH ? new Vector2(-_sprite.Scale.X, _sprite.Scale.Y) : _sprite.Scale;
		GetTree().CurrentScene.AddChild(trail);
	}
}
