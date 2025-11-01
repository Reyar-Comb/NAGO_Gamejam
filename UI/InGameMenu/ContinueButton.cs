using Godot;
using System;

public partial class ContinueButton : Button
{
	public override void _Ready()
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
