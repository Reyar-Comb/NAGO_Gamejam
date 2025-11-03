using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ComboProgressBar : ProgressBar
{
	private class ComboColorSettingsData
	{
		public int RequiredCombo = 0;
		public Color BackgroundColor = new("331111");
		public Color FillColor = new("706666");
	}
	private readonly static List<ComboColorSettingsData> ComboColorSettingsDataList = new()
	{
		new(),
		new()
		{
			RequiredCombo = 5,
			FillColor = new("ad743e")
		},
		new()
		{
			RequiredCombo = 10,
			FillColor = new("b32424")
		},
		new()
		{
			RequiredCombo = 20,
			FillColor = new("ff1a1a")
		}
	};
	[Export] public float BaseDropRate = 5f;
	[Export] public float MaxDropRate = 40f;
	[Export] public float DropRateIncreasePerCombo = 2f;
	public float CurrentDropRate => Mathf.Min(
		BaseDropRate + GameData.Instance.Combo * DropRateIncreasePerCombo,
		MaxDropRate);
	private bool _isRunning = false;
	public override void _Process(double delta)
	{
		if (_isRunning)
		{
			Value -= CurrentDropRate * (float)delta;
		}
		if (Value <= 0 && _isRunning)
		{
			_isRunning = false;
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.ComboBoostEnded);
			GameData.Instance.Combo = 0;
		}
	}

	public void Run()
	{
		ComboColorSettingsData nextComboColorSettingsData =
		ComboColorSettingsDataList
			.Where(x => x.RequiredCombo <= GameData.Instance.Combo)
			.LastOrDefault();
		if (GameData.Instance.Combo <= 0) return;
		_isRunning = true;
		Value = MaxValue;
	}
}
