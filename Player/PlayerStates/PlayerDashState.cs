using Godot;
using System;

public partial class PlayerDashState : State
{
	private Player _player;
	private StaminaBar _staminaBar;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_staminaBar = Storage.GetNode<StaminaBar>("StaminaBar");
	}

	protected override void Enter()
	{
		GD.Print("Player has entered Dash State.");
	}

	protected override void FrameUpdate(double delta)
	{
		if (Input.IsActionJustReleased("Dash") || _staminaBar.CurrentStamina <= 0)
		{
			AskTransit("Walk");
		}
	}
}
