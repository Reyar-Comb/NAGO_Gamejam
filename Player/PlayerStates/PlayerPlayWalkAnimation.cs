using Godot;
using System;

public partial class PlayerPlayWalkAnimation : State
{
	private float AnimationSpeedMultiplier => Storage.GetVariant<float>("AnimationSpeedMultiplier");
	private Player _player;
	private AnimatedSprite2D _sprite;

	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
	}

	protected override void FrameUpdate(double delta)
	{
		DecideAnimationPlay();
	}

	private void TryPlay(string animationName)
	{
		if (_player.CurrentCuisine is not null)
			animationName = "HoldCuisine" + animationName;
		if (_sprite.Animation != animationName)
			_sprite.Play(animationName, AnimationSpeedMultiplier);
	}
	private void DecideAnimationPlay()
	{
		PlayerMoveControl moveControl = Parent as PlayerMoveControl;
		string lastDirection = moveControl.GetLastDirection();
		switch (lastDirection)
		{
			case "Up":
				TryPlay("WalkUp");
				break;
			case "Down":
				TryPlay("WalkDown");
				break;
			case "Left" or "Right":
				TryPlay("WalkHorizontal");
				break;
			default:
				break;
		}
	}
}
