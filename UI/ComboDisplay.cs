using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ComboDisplay : Label
{
	private bool _reached10 = false;
	private class LabelSettingsData
	{
		public int requiredCombo = 0;
		public int fontSize = 70;
		public Color fontColor = Colors.Gray;
		public int outlineSize = 5;
		public Color outlineColor = Colors.DarkGray;
		public int shadowSize = 20;
		public Color shadowColor = Colors.DarkGray;
	}
	private readonly static List<LabelSettingsData> LabelSettingsDataList = new()
	{
		new(),
		new()
		{
			requiredCombo = 5,
			fontSize = 80,
			fontColor = Colors.Orange,
			outlineColor = Colors.DarkOrange,
			shadowColor = Colors.DarkGray with { A = 0.5f }
		},
		new()
		{
			requiredCombo = 10,
			fontSize = 95,
			fontColor = Colors.Red,
			outlineColor = Colors.Orange,
			shadowColor = Colors.DarkRed with { A = 0.8f }
		},
		new()
		{
			requiredCombo = 20,
			fontSize = 105,
			fontColor = Colors.Crimson,
			outlineColor = Colors.Red,
			shadowColor = Colors.Orange with { A = 0.9f }
		}
	};
	public void ReceiveCombo(float newCombo)
	{
		LabelSettingsData nextLabelSettingsData =
		LabelSettingsDataList
			.Where(x => x.requiredCombo <= newCombo)
			.LastOrDefault();
		LabelSettings.FontSize = nextLabelSettingsData.fontSize;
		LabelSettings.FontColor = nextLabelSettingsData.fontColor;
		LabelSettings.OutlineSize = nextLabelSettingsData.outlineSize;
		LabelSettings.OutlineColor = nextLabelSettingsData.outlineColor;
		LabelSettings.ShadowSize = nextLabelSettingsData.shadowSize;
		LabelSettings.ShadowColor = nextLabelSettingsData.shadowColor;
		if (newCombo >= 10 && !_reached10)
        {
			_reached10 = true;
			GetTree().CreateTimer(10f).Timeout += () =>
			{
				GameData.Instance.Combo = 0;
				_reached10 = false;
			};
        }
	}
}
