using Godot;
using System;

public partial class PlayerWalkState : State
{
	private Player _player;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
	}

	protected override void Enter()
	{
		GD.Print("Player has entered Walk State.");
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
