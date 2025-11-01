using Godot;
using System;

public partial class CustomerWalkAnimationState : State
{
	private AnimatedSprite2D _animatedSprite;
	private Customer _customer;

	protected override void ReadyBehavior()
	{
		_animatedSprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_customer = Storage.GetNode<Customer>("Customer");
	}

	protected override void Enter()
	{
		GD.Print("Customer has entered Walk Animation State.");
	}
	protected override void FrameUpdate(double delta)
	{
		if (_customer.Velocity.X > 200)
		{
			if (_animatedSprite.Animation != "WalkRight")
				_animatedSprite.Play("WalkRight");
		}
		else if (_customer.Velocity.X < -200)
		{
			if (_animatedSprite.Animation != "WalkLeft")
				_animatedSprite.Play("WalkLeft");
		}
		else if (_customer.Velocity.Y < 0)
		{
			if (_animatedSprite.Animation != "WalkUp")
				_animatedSprite.Play("WalkUp");
		}
		else if (_customer.Velocity.Y > 0)
		{
			if (_animatedSprite.Animation != "WalkDown")
				_animatedSprite.Play("WalkDown");
		}
		else if (_customer.Velocity == Vector2.Zero)
		{
			if (_customer.IsSeated)
			{
				AskTransit("Sit");
				return;
			}
		}
	}
}
