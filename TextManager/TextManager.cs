using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

public partial class TextManager : Node
{
	public static TextManager Instance { get; private set; }
	public Control DialoguePanel;
	public TextureRect PlayerProfile;
	public Texture2D PlayerProfileHappy;
	public Texture2D PlayerProfileSad;
	public Texture2D PlayerProfileNormal;
	public TextureRect ChefProfile;
	public Label DialogueTextLabel;
	public Label SpeakerNameLabel;
	public Label TipLabel;
	public Button ExitButton;
	public Button UploadButton;
	public LineEdit NameInput;
	public MarginContainer TextMarginContainer;
	public string CurrentDialogueScene = "";
	private float _ePressedTime = 0f;
	private bool _isTextShowing = false;
	[Serializable]
	public class DialogueLine
	{
		public string Id { get; set; }
		public string Side { get; set; }
		public string Text { get; set; }
		public string SpeakerName { get; set; }
		public string Impression { get; set; }
	}
	public DialogueLine[] Lines;
	public int Index = 0;

	public override void _Ready()
	{
		// 单例模式
		if (Instance == null)
		{
			Instance = this;
			// 确保切换场景时不被销毁
			ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			QueueFree();
		}

		GetTextSceneNodes();

		PlayerProfileHappy = GD.Load<Texture2D>("res://Assets/Character/PlayerHappy.png");
		PlayerProfileSad = GD.Load<Texture2D>("res://Assets/Character/PlayerSad.png");
		PlayerProfileNormal = GD.Load<Texture2D>("res://Assets/Character/PlayerProfile.png");
		ExitButton = TextScene.Instance.GetNode<Button>("%ExitButton");
		ExitButton.Visible = false;
		UploadButton = TextScene.Instance.GetNode<Button>("%UploadButton");
		NameInput = TextScene.Instance.GetNode<LineEdit>("%NameInput");
		UploadButton.Visible = false;
		NameInput.Visible = false;
		UploadButton.Pressed += OnUploadButtonPressed;
		AudioManager.Instance.LoadSFX("ShowText", "res://Assets/SoundFX/Click.mp3");
		TipAnimation();
	}
	private void StartDialogue()
	{
		if (_isTextShowing)
			return;
		Index = 0;
		_isTextShowing = true;
		ShowText();
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.DialogueStarted);
	}
	private void GetTextSceneNodes()
	{
		TextScene textScene = TextScene.Instance;
		PlayerProfile = textScene.GetNode<TextureRect>("%PlayerProfile");
		ChefProfile = textScene.GetNode<TextureRect>("%ChefProfile");
		DialogueTextLabel = textScene.GetNode<Label>("%DialogueTextLabel");
		SpeakerNameLabel = textScene.GetNode<Label>("%SpeakerNameLabel");
		TipLabel = textScene.GetNode<Label>("%TipLabel");
	}
	public void ChangePlayerImpresstion(String impression)
	{
		switch (impression)
		{
			case "Happy":
				PlayerProfile.Texture = PlayerProfileHappy;
				break;
			case "Sad":
				PlayerProfile.Texture = PlayerProfileSad;
				break;
			default:
				// 默认表情
				PlayerProfile.Texture = PlayerProfileNormal;
				break;
		}
	}
	private void LoadLines(string path, string scene)
	{
		if (_isTextShowing)
			return;
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		var jsonText = file.GetAsText();
		file.Close();
		var json = JsonSerializer.Deserialize<Dictionary<string, DialogueLine[]>>(jsonText);
		Lines = json[scene];
		CurrentDialogueScene = scene;
	}
	public void RunLines(string path, string scene)
	{
		if (_isTextShowing)
			return;
		LoadLines(path, scene);
		StartDialogue();

		if (scene == "GameOverScene")
		{
			ChangePlayerImpresstion("Sad");
		}
		else
		{
			ChangePlayerImpresstion("Normal");
		}
	}
	public void EndDialogue()
	{
		if (CurrentDialogueScene == "GameOverScene")
		{
			return;
		}
		if (!_isTextShowing)
			return;

		Lines = null;
		Index = 0;
		_isTextShowing = false;
		TextScene.Instance.Visible = false;
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.DialogueEnded);
	}
	private async void ShowText()
	{
		AudioManager.Instance.PlaySFX("ShowText");
		if (!_isTextShowing || Lines is null || Index >= Lines.Length)
		{
			EndDialogue();
			return;
		}

		_isTextShowing = true;
		TextScene.Instance.Visible = true;
		var line = Lines[Index];
		Tween tween = CreateTween();
		if (line.Side == "Left")
		{
			PlayerProfile.Modulate = new Color(1, 1, 1, 1);
			ChefProfile.Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
		}
		else if (line.Side == "Right")
		{
			PlayerProfile.Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
			ChefProfile.Modulate = new Color(1, 1, 1, 1);
		}
		if (line.Side == "Left") ChangePlayerImpresstion(line.Impression);
		DialogueTextLabel.Text = line.Text;
		if (DialogueTextLabel.Text.Contains("#"))
		{
			string temp = "";
			for (int i = 0; i < DialogueTextLabel.Text.Length; i++)
			{
				if (DialogueTextLabel.Text[i] == '&')
				{
					int min = (int)GameData.Instance.TimePassed / 60;
					int sec = (int)GameData.Instance.TimePassed % 60;
					temp += min.ToString("0") + ":" + sec.ToString("00");
				}
				else if (DialogueTextLabel.Text[i] == '#')
				{
					temp += GameData.Instance.Score.ToString("0");
				}
				else
				{
					temp += DialogueTextLabel.Text[i];
				}
			}
			DialogueTextLabel.Text = temp;
		}

		if (DialogueTextLabel.Text.Contains("下次一定"))
		{
			ExitButton.Visible = true;
			UploadButton.Visible = true;
			NameInput.Visible = true;
			ShowInput();
		}
		DialogueTextLabel.VisibleRatio = 0f;
		SpeakerNameLabel.Text = line.SpeakerName;
		float durationFactor = 0.05f;
		tween.TweenProperty(DialogueTextLabel, "visible_ratio", 1f, line.Text.Length * durationFactor);

		while (true)
		{
			if (!_isTextShowing) return;

			if (IsSkipping())
			{
				tween.Kill();
				DialogueTextLabel.VisibleRatio = 1f;

				await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
				break;
			}
			else if (DialogueTextLabel.VisibleRatio >= 0.999f)
			{
				break;
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			if (Lines == null || Index >= Lines.Length)
			{
				EndDialogue();
				return;
			}
		}

		WaitAdvance();
	}

	public async void WaitAdvance()
	{
		while (true)
		{
			if (!_isTextShowing) return;

			if (IsSkipping())
			{
				await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
				break;
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

			if (Lines == null || Index >= Lines.Length)
			{
				EndDialogue();
				return;
			}
		}
		Index++;
		ShowText();
	}
	private bool IsSkipping()
	{
		return Input.IsMouseButtonPressed(MouseButton.Left) || Input.IsActionJustPressed("Interact");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Skip") && _isTextShowing)
		{
			_ePressedTime += (float)delta;
			if (_ePressedTime >= 1.2f)
			{
				EndDialogue();
			}
		}
	}

	public void TipAnimation()
	{
		Tween tween = CreateTween();
		tween.SetLoops();
		tween.TweenProperty(TipLabel, "modulate:a", 0f, 1f).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(TipLabel, "modulate:a", 1f, 1f).SetEase(Tween.EaseType.InOut);
	}

	private void ShowInput()
	{
		
		NameInput.Visible = true;
		NameInput.Editable = true;  // 确保可编辑
		NameInput.GrabFocus();      // 强制聚焦
		NameInput.Text = "";        // 清空文本
		GD.Print(NameInput.HasFocus());
	}
	
	public void OnUploadButtonPressed()
	{
		string playerName = NameInput.Text.Trim();
		if (string.IsNullOrEmpty(playerName))
		{
			GD.Print("Player name cannot be empty.");
			NameInput.Text = "";
			NameInput.PlaceholderText = "请输入有效的名字";
			return;
		}
		int min = (int)GameData.Instance.TimePassed / 60;
		int sec = (int)GameData.Instance.TimePassed % 60;
		string time = min.ToString("0") + ":" + sec.ToString("00");

		string jsondata = JsonSerializer.Serialize(new Dictionary<string, object>
		{
			{ "username", playerName },
			{ "score", GameData.Instance.Score },
			{ "time", time }
		});
		var httpRequest = new HttpRequest();
		AddChild(httpRequest);
		string[] headers = new string[] { "Content-Type: application/json" };
		httpRequest.Request("http://47.115.77.27:5067/upload", headers, HttpClient.Method.Post, jsondata);
		httpRequest.RequestCompleted += (long result, long responseCode, string[] responseHeaders, byte[] body) =>
		{
			GD.Print("Upload Response Code: " + responseCode);
			if (responseCode == 200 || responseCode == 201)
			{
				GD.Print("Score uploaded successfully!");
				UploadButton.Disabled = true;
				NameInput.Text = "";
				NameInput.PlaceholderText = "分数已上传~";
				UploadButton.Text = "QwQ";
			}
			else if (responseCode == 400)
			{
				GD.PrintErr("Invalid data format.");
				NameInput.Text = "";
				NameInput.PlaceholderText = "名称包含非法字符";
			}
			else if (responseCode == 401)
			{
				GD.PrintErr("Unauthorized request.");
				NameInput.Text = "";
				NameInput.PlaceholderText = "名称应在3-20字符之间";
			}
			else
			{
				GD.PrintErr("Failed to upload score.");
				NameInput.Text = "";
				NameInput.PlaceholderText = "网络错误, 上传失败";
			}
		};


	}
}
