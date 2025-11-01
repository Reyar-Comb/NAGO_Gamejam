using Godot;
using System;

public partial class TestMainScene : Node2D
{
	[Export] CustomerManager CustomerManager;
	public override void _Ready()
	{

	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			CustomerManager.SpawnCustomer();
		}
	}
}
