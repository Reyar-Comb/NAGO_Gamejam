using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ComboDisplay : Label
{
	private bool _reached10 = false;
	private class LabelSettingsData
	{
		public int RequiredCombo = 0;
		public int FontSize = 70;
		public Color FontColor = Colors.Gray;
		public int OutlineSize = 5;
		public Color OutlineColor = Colors.DarkGray;
		public int ShadowSize = 20;
		public Color ShadowColor = Colors.DarkGray;
	}
	private readonly static List<LabelSettingsData> LabelSettingsDataList = new()
	{
		new(),
		new()
		{
			RequiredCombo = 5,
			FontSize = 80,
			FontColor = Colors.Orange,
			OutlineColor = Colors.DarkOrange,
			ShadowColor = Colors.DarkGray with { A = 0.5f }
		},
		new()
		{
			RequiredCombo = 10,
			FontSize = 95,
			FontColor = Colors.Red,
			OutlineColor = Colors.Orange,
			ShadowColor = Colors.DarkRed with { A = 0.8f }
		},
		new()
		{
			RequiredCombo = 20,
			FontSize = 105,
			FontColor = Colors.Crimson,
			OutlineColor = Colors.Red,
			ShadowColor = Colors.Orange with { A = 0.9f }
		}
	};
	public void ReceiveCombo(float newCombo)
	{
		LabelSettingsData nextLabelSettingsData =
		LabelSettingsDataList
			.Where(x => x.RequiredCombo <= newCombo)
			.LastOrDefault();
		LabelSettings.FontSize = nextLabelSettingsData.FontSize;
		LabelSettings.FontColor = nextLabelSettingsData.FontColor;
		LabelSettings.OutlineSize = nextLabelSettingsData.OutlineSize;
		LabelSettings.OutlineColor = nextLabelSettingsData.OutlineColor;
		LabelSettings.ShadowSize = nextLabelSettingsData.ShadowSize;
		LabelSettings.ShadowColor = nextLabelSettingsData.ShadowColor;
		if (newCombo >= 10 && !_reached10)
		{
			_reached10 = true;
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.ComboReached10);
		}
		if (newCombo < 10)
		{
			_reached10 = false;
		}
	}
}
