using Godot;
using System;

public partial class SettingsButton : ScaleButton
{
	protected override void ReadyBehavior()
	{
		ProcessMode = ProcessModeEnum.Always;
		Pressed += () =>
		{
			AudioManager.Instance.PlaySFX("ButtonClick");
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.InGameMenuSettingsToggled);
		};
	}
}
