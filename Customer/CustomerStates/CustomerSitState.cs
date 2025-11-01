using Godot;
using System;

public partial class CustomerSitState : State
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

		GD.Print("Customer has entered Sit State.");
		switch (_customer.TargetChairType)
		{
			case Chair.ChairType.Up:
				_customer.ZIndex = 1;
				_animatedSprite.GlobalPosition = _customer.TargetChairPosition + new Vector2(0, 85) + new Vector2(0, -100);
				_animatedSprite.Stop();
				_animatedSprite.Play("SitUp");
				break;
			case Chair.ChairType.Left:
				_animatedSprite.GlobalPosition = _customer.TargetChairPosition + new Vector2(160, -200) + new Vector2(0, -80);
				_animatedSprite.Stop();
				_animatedSprite.Play("SitRight");
				_animatedSprite.FlipH = true;
				break;
			case Chair.ChairType.Right:
				_animatedSprite.GlobalPosition = _customer.TargetChairPosition + new Vector2(-160, -200) + new Vector2(0, -80);
				_animatedSprite.Stop();
				_animatedSprite.Play("SitRight");
				_animatedSprite.FlipH = false;
				break;
			default:
				_animatedSprite.Stop();
				_animatedSprite.Play("SitUp");
				break;
		}
	}
}
