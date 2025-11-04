using Godot;
using System;

public partial class TextScene : CanvasLayer
{
	public static TextScene Instance { get; private set; }
	[Export] public LineEdit NameInput;
	public override void _Ready()
	{
		Instance = this;
		NameInput.GrabFocus();
	}
}
