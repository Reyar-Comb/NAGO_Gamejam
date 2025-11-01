using Godot;
using System;
using System.Collections;

public partial class WaterPuddle : Area2D
{
	public override void _Ready()
    {
		GetTree().CreateTimer(20f).Timeout += QueueFree;
    }
}
