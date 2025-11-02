using Godot;
using System;

public partial class InGameMenu : CanvasLayer
{
	private bool _isSettingsOpen = false;
	public override void _Ready()
	{
		Visible = false;
		ProcessMode = ProcessModeEnum.Always;
		SignalBus.Instance.InGameMenuSettingsToggled += OnInGameMenuSettingsToggled;
	}
	public override void _ExitTree()
	{
		SignalBus.Instance.InGameMenuSettingsToggled -= OnInGameMenuSettingsToggled;
	}
	public override void _Process(double delta)
	{
		if (_isSettingsOpen) return;
        if (Input.IsActionJustPressed("ToggleMenu"))
		{
			GetTree().Paused = !GetTree().Paused;
			Visible = GetTree().Paused;
		}
    }
	private void OnInGameMenuSettingsToggled()
	{
		_isSettingsOpen = !_isSettingsOpen;
	}
}
