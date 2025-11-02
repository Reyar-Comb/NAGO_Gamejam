using Godot;
using System;

public partial class InGameMenuSettings : Control
{
	private TextureButton _bgmTickButton;
	private TextureButton _bgmTickedButton;
	private TextureButton _sfxTickButton;
	private TextureButton _sfxTickedButton;
	private HSlider _bgmSlider;
	private HSlider _sfxSlider;
	private TextureProgressBar _bgmProgressBar;
	private TextureProgressBar _sfxProgressBar;
	public override void _Ready()
	{
		InitializeNodeReferences();
		InitializeSignals();
		AudioManager.Instance.LoadSFX("ButtonHover", "res://Assets/SoundFX/hover2.mp3");
		AudioManager.Instance.LoadSFX("ButtonClick", "res://Assets/SoundFX/click.mp3");
		SignalBus.Instance.InGameMenuSettingsToggled += OnInGameMenuSettingsToggled;
	}
	public override void _Process(double delta)
	{
		if (Visible && Input.IsActionJustPressed("ToggleMenu"))
		{
			AudioManager.Instance.PlaySFX("ButtonClick");
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.InGameMenuSettingsToggled);
		}
	}
	public override void _ExitTree()
	{
		SignalBus.Instance.InGameMenuSettingsToggled -= OnInGameMenuSettingsToggled;
	}
	private void OnInGameMenuSettingsToggled()
	{
		Visible = !Visible;
	}
	private void InitializeNodeReferences()
	{
		_bgmTickButton = GetNode<TextureButton>("BGMTickButton");
		_bgmTickedButton = GetNode<TextureButton>("BGMTickedButton");
		_sfxTickButton = GetNode<TextureButton>("SFXTickButton");
		_sfxTickedButton = GetNode<TextureButton>("SFXTickedButton");
		_bgmSlider = GetNode<HSlider>("BGMSlider");
		_sfxSlider = GetNode<HSlider>("SFXSlider");
		_bgmProgressBar = GetNode<TextureProgressBar>("BGMProgressBar");
		_sfxProgressBar = GetNode<TextureProgressBar>("SFXProgressBar");
	}
	private void InitializeSignals()
	{
		_bgmTickButton.Pressed += () =>
		{
			AudioManager.Instance.PlaySFX("ButtonClick");
			_bgmTickedButton.Visible = true;
			AudioServer.SetBusMute(AudioServer.GetBusIndex("BGM"), false);
		};
		_bgmTickedButton.Pressed += () =>
		{
			AudioManager.Instance.PlaySFX("ButtonClick");
			_bgmTickedButton.Visible = false;
			AudioServer.SetBusMute(AudioServer.GetBusIndex("BGM"), true);
		};
		_sfxTickButton.Pressed += () =>
		{
			AudioManager.Instance.PlaySFX("ButtonClick");
			_sfxTickedButton.Visible = true;
			AudioServer.SetBusMute(AudioServer.GetBusIndex("SFX"), false);
		};
		_sfxTickedButton.Pressed += () =>
		{
			AudioManager.Instance.PlaySFX("ButtonClick");
			_sfxTickedButton.Visible = false;
			AudioServer.SetBusMute(AudioServer.GetBusIndex("SFX"), true);
		};
		_bgmProgressBar.Value = _bgmSlider.Value;
		_bgmSlider.ValueChanged += (value) =>
		{
			AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("BGM"), (float)value / 100f);
			_bgmProgressBar.Value = value;
		};
		_sfxProgressBar.Value = _sfxSlider.Value;
		_sfxSlider.ValueChanged += (value) =>
		{
			AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("SFX"), (float)value / 100f);
			_sfxProgressBar.Value = value;
		};
	}
}
