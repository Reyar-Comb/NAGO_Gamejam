using Godot;
using System;

public partial class ExitButton : ScaleButton
{
	protected override void ReadyBehavior()
	{
		ProcessMode = ProcessModeEnum.Always;
		Pressed += () => GetTree().Quit();
	}
}
