using Godot;
using System;

public partial class TextScene : CanvasLayer
{
	public static TextScene Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
	}
}
