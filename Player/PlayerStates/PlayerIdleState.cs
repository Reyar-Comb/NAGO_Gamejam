using Godot;
using System;

public partial class PlayerIdleState : State
{
    private Player _player;
    private AnimatedSprite2D _sprite;
    protected override void ReadyBehavior()
    {
        _player = Storage.GetNode<Player>("Player");
        _sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
        _sprite.Play("IdleDown");
    }
    private void TryPlay(string animationName)
    {
        string prefix = _player.CurrentCuisine is null ? "Idle" : "HoldCuisine";
        string animationNameWithPrefix = prefix + animationName;
        if (_sprite.Animation != animationNameWithPrefix)
            _sprite.Play(animationNameWithPrefix);
    }
    private void DecideAnimationPlay()
    {
		PlayerMoveControl moveControl = Parent as PlayerMoveControl;
		string lastDirection = string.IsNullOrEmpty(moveControl.GetLastDirection())
            ? Storage.GetVariant<string>("LastRemovedDirection")
            : moveControl.GetLastDirection();
        switch (lastDirection)
        {
            case "Up":
                TryPlay("Up");
                break;
            case "Down":
                TryPlay("Down");
                break;
            case "Left" or "Right":
                TryPlay("Horizontal");
                break;
            default:
                break;
        }
    }
    protected override void Enter()
    {
        GD.Print("Player has entered Idle State.");
        DecideAnimationPlay();
    }

    protected override void FrameUpdate(double delta)
    {
        DecideAnimationPlay();
        if (!Mathf.IsZeroApprox(_player.Velocity.Length()))
        {
            AskTransit("Walk");
        }
        if (Input.IsActionJustPressed("Dash"))
            AskTransit("Dash");
    }
}
