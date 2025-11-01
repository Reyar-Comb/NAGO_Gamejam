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
        // 如果速度接近零，检查是否到达座位
        if (_customer.Velocity.LengthSquared() < 10f)
        {
            if (_customer.IsSeated && !_customer.IsLeaving)
            {
                AskTransit("Sit");
                return;
            }
            return;
        }
        
        // 根据速度的主要方向播放动画
        Vector2 velocity = _customer.Velocity;
        
        // 判断水平还是垂直移动占主导（比较绝对值）
        if (Mathf.Abs(velocity.X) > Mathf.Abs(velocity.Y))
        {
            // 水平移动为主
            if (velocity.X > 0)
            {
                if (_animatedSprite.Animation != "WalkRight")
                    _animatedSprite.Play("WalkRight");
            }
            else
            {
                if (_animatedSprite.Animation != "WalkLeft")
                    _animatedSprite.Play("WalkLeft");
            }
        }
        else
        {
            // 垂直移动为主
            if (velocity.Y > 0)
            {
                if (_animatedSprite.Animation != "WalkDown")
                    _animatedSprite.Play("WalkDown");
            }
            else
            {
                if (_animatedSprite.Animation != "WalkUp")
                    _animatedSprite.Play("WalkUp");
            }
        }
    }
}
