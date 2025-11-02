using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public partial class ResponsiveButton : TextureButton
{
	public Tween buttonTween;
	public bool PlayedSFX = false;
	private Button AboutButton;
	[Export] public Control AboutMenu;
	[Export] public Control SettingsMenu;
	private TextureButton BGMTickButton;
	private TextureButton BGMTickedButton;
	private TextureButton SFXTickButton;
	private TextureButton SFXTickedButton;
	private HSlider BGMSlider;
	private TextureProgressBar BGMProgressBar;
	private HSlider SFXSlider;
	private TextureProgressBar SFXProgressBar;
	public override void _Ready()
	{
		MouseEntered += OnMouseEnter;
		MouseExited += OnMouseExit;
		Pressed += OnMousePressed;
		PivotOffset = GetSize() / 2;
		AudioManager.Instance.LoadSFX("ButtonHover", "res://Assets/SoundFX/hover2.mp3");
		AudioManager.Instance.LoadSFX("ButtonClick", "res://Assets/SoundFX/click.mp3");

		if (AboutMenu != null)
		{
			AboutMenu.Visible = false;
			AboutButton = GetNode<Button>("../../About/BackButton");
			AboutButton.Pressed += () =>
			{
				AudioManager.Instance.PlaySFX("ButtonClick");
				AboutMenu.Visible = false;
				AboutButton.Disabled = true;
				AboutButton.Modulate = new Color(1, 1, 1, 0f);
			};
		}
		
		if (SettingsMenu != null)
		{
			SettingsMenu.Visible = false;
			BGMTickButton = GetNode<TextureButton>("../../Settings/BGMTickButton");
			BGMTickedButton = GetNode<TextureButton>("../../Settings/BGMTickedButton");
			BGMTickButton.Pressed += () =>
			{
				AudioManager.Instance.PlaySFX("ButtonClick");
				BGMTickedButton.Visible = true;
				AudioServer.SetBusMute(AudioServer.GetBusIndex("BGM"), false);
			};
			BGMTickedButton.Pressed += () =>
			{
				AudioManager.Instance.PlaySFX("ButtonClick");
				BGMTickedButton.Visible = false;
				AudioServer.SetBusMute(AudioServer.GetBusIndex("BGM"), true);
			};
			SFXTickButton = GetNode<TextureButton>("../../Settings/SFXTickButton");
			SFXTickedButton = GetNode<TextureButton>("../../Settings/SFXTickedButton");
			SFXTickButton.Pressed += () =>
			{
				AudioManager.Instance.PlaySFX("ButtonClick");
				SFXTickedButton.Visible = true;
				AudioServer.SetBusMute(AudioServer.GetBusIndex("SFX"), false);
			};
			SFXTickedButton.Pressed += () =>
			{
				AudioManager.Instance.PlaySFX("ButtonClick");
				SFXTickedButton.Visible = false;
				AudioServer.SetBusMute(AudioServer.GetBusIndex("SFX"), true);
			};
			BGMSlider = GetNode<HSlider>("../../Settings/BGMSlider");
			BGMProgressBar = GetNode<TextureProgressBar>("../../Settings/BGMProgressBar");
			BGMProgressBar.Value = BGMSlider.Value;
			BGMSlider.ValueChanged += (value) =>
			{
				AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("BGM"), (float)value / 100f);
				BGMProgressBar.Value = value;
			};
			SFXSlider = GetNode<HSlider>("../../Settings/SFXSlider");
			SFXProgressBar = GetNode<TextureProgressBar>("../../Settings/SFXProgressBar");
			SFXProgressBar.Value = SFXSlider.Value;
			SFXSlider.ValueChanged += (value) =>
			{
				AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("SFX"), (float)value / 100f);
				SFXProgressBar.Value = value;
			};
		}
	}

	public void OnMouseEnter()
	{
		
		GD.Print("Mouse Entered");
		if (buttonTween != null)
		{
			buttonTween.Kill();
		}
		if (!PlayedSFX)
		{
			AudioManager.Instance.PlaySFX("ButtonHover");
			
			PlayedSFX = true;
		}
		buttonTween = GetTree().CreateTween();
		buttonTween.SetTrans(Tween.TransitionType.Bounce);
		buttonTween.TweenProperty(this, "scale", new Vector2(0.85f, 0.85f), 0.2f).SetEase(Tween.EaseType.Out);
	}

	public void OnMouseExit()
	{

		if (buttonTween != null)
		{
			buttonTween.Kill();
		}
		PlayedSFX = false;
		
		buttonTween = GetTree().CreateTween();
		buttonTween.SetTrans(Tween.TransitionType.Bounce);
		buttonTween.TweenProperty(this, "scale", new Vector2(0.75f, 0.75f), 0.2f).SetEase(Tween.EaseType.Out);
	}

	public async void OnMousePressed()
	{
		if (buttonTween != null)
		{
			buttonTween.Kill();
		}
		buttonTween = GetTree().CreateTween();
		buttonTween.SetTrans(Tween.TransitionType.Bounce);
		buttonTween.TweenProperty(this, "scale", new Vector2(0.7f, 0.7f), 0.1f).SetEase(Tween.EaseType.Out);
		buttonTween.TweenProperty(this, "scale", new Vector2(0.85f, 0.85f), 0.1f).SetEase(Tween.EaseType.Out);
		AudioManager.Instance.PlaySFX("ButtonClick");

		if (this.Name == "StartButton")
		{
			AudioManager.Instance.StopBGM(1f);
			await SceneManager.Instance.ChangeScene(GD.Load<PackedScene>("res://MainScene/test/testMainScene.tscn"));
		}
		else if (this.Name == "QuitButton")
		{
			AudioManager.Instance.StopBGM(1f);
			await ToSignal(GetTree().CreateTimer(2f), "timeout");
			GetTree().Quit();
		}
		else if (this.Name == "AboutButton")
		{
			await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
			AboutMenu.Visible = true;
			AboutButton.Disabled = false;
		}
		else if (this.Name == "SettingsButton")
		{
			await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
			SettingsMenu.Visible = true;

		}



	}
	
	public override void _Process(double delta)
	{
		if (SettingsMenu != null && SettingsMenu.Visible == true && Input.IsActionJustPressed("ui_cancel"))
		{
			SettingsMenu.Visible = false;
		}
		else if (AboutMenu != null && AboutMenu.Visible == true && Input.IsActionJustPressed("ui_cancel"))
		{
			AboutMenu.Visible = false;
			AboutButton.Disabled = true;
			AboutButton.Modulate = new Color(1, 1, 1, 0f);
		}
		
	}
}
