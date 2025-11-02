using Godot;
using System;

public partial class TestMainScene : Node2D
{
	[Export] CustomerManager CustomerManager;
	public override void _Ready()
	{
		AudioManager.Instance.LoadBGM("BGM", "res://Assets/SoundFX/bgm.mp3");
		AudioManager.Instance.LoadSFX("Click", "res://Assets/SoundFX/click.mp3");

		AudioManager.Instance.LoadBGM("Routine", "res://Assets/SoundFX/lofi8bit.mp3");
		TextScene.Instance.Visible = true;
		TextManager.Instance.RunLines("res://TextManager/test.json", "StartScene");
		AudioManager.Instance.PlayBGM("Routine", 29.54f, 44.31f);
		SignalBus.Instance.Connect(SignalBus.SignalName.DialogueEnded, new Callable(this, MethodName.OnStartDialugueEnded));
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			SceneManager.Instance.ReloadRestaurantScene();
		}
	}

	public void OnStartDialugueEnded()
	{
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameStart);
	}
}
