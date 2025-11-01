using Godot;
using System;

public partial class PlayerSlipState : State
{
	private bool CanDash
    {
        get => Storage.GetVariant<bool>("CanDash");
		set => Storage.SetVariant("CanDash", value);
    }
	private Player _player;
	private AnimatedSprite2D _sprite;

	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_sprite = Storage.GetNode<AnimatedSprite2D>("AnimatedSprite");
	}

	protected override void Enter()
	{
		GD.Print("Player has entered Slip State.");
		GetTree().CreateTimer(3f).Timeout += () => AskTransit("Idle");
		_player.CurrentCuisine = null;
		CanDash = false;
	}
    protected override void Exit()
    {
        CanDash = true;
    }
}
