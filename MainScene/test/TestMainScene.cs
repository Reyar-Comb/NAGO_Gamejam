using Godot;
using System;

public partial class TestMainScene : Node2D
{
	[Export] CustomerManager CustomerManager;
	public override void _Ready()
	{
		AudioManager.Instance.LoadBGM("BGM", "res://Assets/SoundFX/bgm.mp3");
		AudioManager.Instance.LoadSFX("Click", "res://Assets/SoundFX/click.mp3");

		AudioManager.Instance.PlayBGM("BGM");
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			GameData.Instance.TimePassed += 60;
		}
	}
}
