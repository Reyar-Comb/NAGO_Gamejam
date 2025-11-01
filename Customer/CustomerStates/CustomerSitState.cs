using Godot;
using System;

public partial class CustomerSitState : State
{
	private AnimatedSprite2D _animatedSprite;
	private Customer _customer;
	private CollisionShape2D _collisionShape;
	private Vector2 _originalAnimatedSpritePosition;
	private Vector2 _originalCollisionShapePosition;
	private int _originalZIndex;
	private Vector2 _originalScale;
	private bool _originalFlipH = false;
	protected override void ReadyBehavior()
	{
		_animatedSprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_customer = Storage.GetNode<Customer>("Customer");
		_collisionShape = Storage.GetNode<CollisionShape2D>("CollisionShape2D");
		_customer.CustomerLeft += OnCustomerLeft;
	}
	protected override void Enter()
	{
		_originalAnimatedSpritePosition = _animatedSprite.GlobalPosition;
		_originalCollisionShapePosition = _collisionShape.GlobalPosition;
		_originalZIndex = _customer.ZIndex;
		_originalScale = _customer.Scale;
		_customer.Scale = new Vector2(0.55f, 0.55f);
		_originalFlipH = _animatedSprite.FlipH;
		switch (_customer.TargetChairType)
		{
			case Chair.ChairType.Up:
				_customer.ZIndex = 1;
				_animatedSprite.GlobalPosition = _customer.TargetChairPosition + new Vector2(0, 85) + new Vector2(0, -100);
				_collisionShape.GlobalPosition = _animatedSprite.GlobalPosition;
				_animatedSprite.Stop();
				_animatedSprite.Play("SitUp");
				break;
			case Chair.ChairType.Left:
				_animatedSprite.GlobalPosition = _customer.TargetChairPosition + new Vector2(160, -200) + new Vector2(0, -70);
				_collisionShape.GlobalPosition = _animatedSprite.GlobalPosition;
				_animatedSprite.Stop();
				_animatedSprite.Play("SitRight");
				_animatedSprite.FlipH = true;
				break;
			case Chair.ChairType.Right:
				_animatedSprite.GlobalPosition = _customer.TargetChairPosition + new Vector2(-160, -200) + new Vector2(0, -80);
				_collisionShape.GlobalPosition = _animatedSprite.GlobalPosition;
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
	private void OnCustomerLeft()
	{
		_customer.Scale = _originalScale;
		_animatedSprite.GlobalPosition = _originalAnimatedSpritePosition;
		_collisionShape.GlobalPosition = _originalCollisionShapePosition;
		_customer.ZIndex = _originalZIndex;
		_animatedSprite.FlipH = _originalFlipH;
		CustomerManager customerManager = GetTree().CurrentScene.GetNode<CustomerManager>("%CustomerManager");
		var randomIndex = GD.Randi() % customerManager.SpawnPositions.Count;
		Vector2 targetPosition = customerManager.SpawnPositions[(int)randomIndex];
		_customer.MoveTo(targetPosition);
		AskTransit("Walk");
	}
}
