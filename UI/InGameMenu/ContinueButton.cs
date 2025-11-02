using Godot;
using System;

public partial class ContinueButton : ScaleButton
{
	protected override void ReadyBehavior()
	{
		Pressed += () =>
		{
			GetTree().Paused = false;
			CanvasLayer owner = Owner as CanvasLayer;
			owner.Visible = false;
			owner.Visible = false;
		};
		ProcessMode = ProcessModeEnum.Always;
	}
}
