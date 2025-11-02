using Godot;
using System;

public partial class RestartButton : ScaleButton
{
	protected override void ReadyBehavior()
	{
		Pressed += GameData.Instance.ResetGameData;
		ProcessMode = ProcessModeEnum.Always;
	}
}
