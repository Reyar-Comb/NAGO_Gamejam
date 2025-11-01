using Godot;
using System;

public partial class TestText : Node2D
{
	public override void _Ready()
	{
		TextManager.Instance.RunLines("res://TextManager/test.json", "scene1");
	}
}
