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
		SignalBus.Instance.Connect(SignalBus.SignalName.GameStart, new Callable(this, MethodName.OnGameStart));
	}
	public override void _Process(double delta)
	{
		// if (Input.IsActionJustPressed("ui_accept"))
		// {
		// 	GameData.Instance.TimePassed += 60;
		// }
		if (GameData.Instance.NegativeViews >= 10)
		{
			GameOver();
		}
	}
	public async void TryPlayRoutineBGM()
	{
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		AudioManager.Instance.PlayBGM("Routine", 29.54f, 44.31f);
	}

	public async void OnStartDialugueEnded()
	{
		AudioManager.Instance.StopBGM();

		SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameStart);
		
	}
	public async void OnGameStart()
	{
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		AudioManager.Instance.StopBGM();
		AudioManager.Instance.SetBGMVolume(AudioManager.Instance.DefaultBGMVolume);
		AudioManager.Instance.PlayBGM("BGM");
		GD.Print(AudioManager.Instance.BGMPlayer.Playing);
	}

	public async void GameOver()
	{
		GetTree().Paused = true;
		this.GetNode<CanvasLayer2>("CanvasLayer").Showw();
		AudioManager.Instance.StopBGM(3f);
		await ToSignal(GetTree().CreateTimer(3f), "timeout");
		await SceneManager.Instance.ChangeScene(GD.Load<PackedScene>("res://MainScene/GameOverScene/GameOver.tscn"));
	}
}
