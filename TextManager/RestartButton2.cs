using Godot;
using System;
using System.Threading.Tasks;

public partial class RestartButton2 : ScaleButton
{
	protected override void ReadyBehavior()
	{
		Pressed += GameData.Instance.ResetGameData;
		Pressed += OnPressed;
		ProcessMode = ProcessModeEnum.Always;
	}

	public async void OnPressed()
	{
		await ToSignal(GetTree().CreateTimer(0.4f), "timeout");
		TextScene.Instance.Visible = false;
		TextManager.Instance.RunLines("res://TextManager/Dialogues.json", "StartScene");
		await SceneManager.Instance.ChangeScene(GD.Load<PackedScene>("res://MainScene/test/testMainScene.tscn"));
		
	}
}
