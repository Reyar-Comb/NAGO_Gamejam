using Godot;
using System;
using System.Threading.Tasks;

public partial class TestMainScene : Node2D
{
	[Export] CustomerManager CustomerManager;
	public override void _Ready()
	{
		AudioManager.Instance.LoadBGM("BGM", "res://Assets/SoundFX/bgm.mp3");
		AudioManager.Instance.LoadSFX("Click", "res://Assets/SoundFX/click.mp3");

		AudioManager.Instance.LoadBGM("Routine", "res://Assets/SoundFX/lofi8bit.mp3");
		TextScene.Instance.Visible = true;
		TextManager.Instance.RunLines("res://TextManager/Dialogues.json", "StartScene");
		TryPlayRoutineBGM();
		SignalBus.Instance.Connect(SignalBus.SignalName.DialogueEnded, new Callable(this, MethodName.OnStartDialugueEnded));
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			SceneManager.Instance.ReloadRestaurantScene();
		}
	}
	public async void TryPlayRoutineBGM()
	{
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		AudioManager.Instance.PlayBGM("Routine", 29.54f, 44.31f);
		
	}

	public async void OnStartDialugueEnded()
	{
		AudioManager.Instance.StopBGM(1f);
		await ToSignal(GetTree().CreateTimer(1f), "timeout");

		SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameStart);
		AudioManager.Instance.PlayBGM("BGM");
	}
}
