using Godot;
using System;
using System.Threading.Tasks;
public partial class StartMenu : Control
{
	[Export] public TextureRect Title;

	[Export] public HttpRequest httpRequest;
	[Export] public Label leaderboardLabel;
	
	private Vector2 TitlePos;
	private Tween _titleTween;

	private Tween _buttonTween;
	public override async void _Ready()
	{
		TitlePos = Title.GlobalPosition;

		AudioManager.Instance.LoadBGM("BGM", "res://Assets/SoundFX/bgm.mp3");
		TitleAnimation();
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		AudioManager.Instance.PlayBGM("BGM");
 		httpRequest.Request("http://47.115.77.27:5067/leaderboard");
		httpRequest.RequestCompleted += OnRequestCompleted;
	}

	public void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print("Response Code: " + responseCode);
		string responseBody = System.Text.Encoding.UTF8.GetString(body);
		GD.Print("Response Body: " + responseBody);
		ShowLeaderboard(responseBody);
		
	}

	public void ShowLeaderboard(string dataText)
	{
		Json json = new Json();
		Error error = json.Parse(dataText);
		if (error != Error.Ok)
		{
			GD.PrintErr("JSON Parse Error: " + error);
			return;
		}
		string text = "";
		Godot.Collections.Array data = json.Data.AsGodotArray();
		for (int i = 0; i < data.Count; i++)
		{
			Godot.Collections.Dictionary entry = data[i].AsGodotDictionary();
			string name = entry["username"].ToString();
			string score = ((int)entry["score"]).ToString();
			score = score.PadLeft(6, ' ');
			string time = entry["time"].ToString();
			time = time.Length > 4 ? time : "0" + time;
			text += $"{i + 1}. {name} \n分数: {score}   时间: {time}\n\n";
		}
		leaderboardLabel.Text = text;
	}
	public void TitleAnimation()
	{
		if (_titleTween != null)
		{
			_titleTween.Kill();  // 停止旧动画
		}
		_titleTween = CreateTween();
		_titleTween.SetTrans(Tween.TransitionType.Sine);
		_titleTween.SetLoops(-1);
		_titleTween.TweenProperty(Title, "global_position", TitlePos - new Vector2(0, -20f), 1f).SetEase(Tween.EaseType.InOut);
		_titleTween.TweenProperty(Title, "global_position", TitlePos - new Vector2(0, 20f), 1f).SetEase(Tween.EaseType.InOut);

	}

	
}
