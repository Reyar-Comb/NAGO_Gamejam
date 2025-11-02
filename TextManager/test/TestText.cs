using Godot;
using System;

public partial class TestText : Node2D
{
	public override void _Ready()
	{
		AudioManager.Instance.LoadBGM("Routine", "res://Assets/SoundFX/lofi8bit.mp3");
		TextManager.Instance.RunLines("res://TextManager/test.json", "StartScene");
		AudioManager.Instance.PlayBGM("Routine", 29.54f, 44.31f);
	}
}
