using Godot;
using System;
using System.Threading.Tasks;

public partial class TestMainScene : Node2D
{
	[Export] CustomerManager CustomerManager;
	// private int _frameInterval = 5;
	// private int _frameCounter = 0;
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
		// Node2D container = GetNode<Node2D>("%CustomerContainer");
		// _frameCounter++;
		// if (_frameCounter % _frameInterval != 0) return;
		// for (int i = 0; i < container.GetChildCount(); i++)
		// {
		// 	for (int j = i + 1; j < container.GetChildCount(); j++)
		// 	{
		// 		Customer customerA = container.GetChild<Customer>(i);
		// 		Customer customerB = container.GetChild<Customer>(j);
		// 		if (customerA.TargetChairPosition.DistanceTo(customerB.TargetChairPosition) < 20f
		// 		&& !customerA.IsLeaving && !customerB.IsLeaving && customerA.IsSeated && customerB.IsSeated)
		// 		{
		// 			GD.Print("FUCK: Collision detected between customers at positions: " +
		// 				customerA.TargetChairPosition + " and " + customerB.TargetChairPosition);
		// 		}
		// 	}
		// }
		if (GameData.Instance.NegativeViews >= GameData.MaxNegativeViewsAllowed)
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
