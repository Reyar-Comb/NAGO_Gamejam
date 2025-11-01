using Godot;
using System;

public partial class RestartButton : Button
{
	public override void _Ready()
	{
		Pressed += GameData.Instance.ResetGameData;
		ProcessMode = ProcessModeEnum.Always;
	}
}
