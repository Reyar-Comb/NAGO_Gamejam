using Godot;
using System;

public partial class GameOver : Control
{
	
	public override void _Ready()
	{
		TextManager.Instance.RunLines("res://TextManager/Dialogues.json", "GameOverScene");
		TextScene.Instance.Visible = true;
		AudioManager.Instance.LoadBGM("GameOver", "res://Assets/SoundFX/lofi8bit.mp3");
		AudioManager.Instance.PlayBGM("GameOver");
	}
}
