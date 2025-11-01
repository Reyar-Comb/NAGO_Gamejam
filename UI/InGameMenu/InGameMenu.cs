using Godot;
using System;

public partial class InGameMenu : CanvasLayer
{
	public override void _Ready()
    {
		Visible = false;
		ProcessMode = ProcessModeEnum.Always;
    }
	public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("OpenMenu"))
		{
			GetTree().Paused = !GetTree().Paused;
			Visible = GetTree().Paused;
		}
    }
}
