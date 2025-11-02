using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ComboDisplay : Label
{
	private class LabelSettingsData
	{
		public int requiredCombo = 0;
		public int fontSize = 70;
		public Color fontColor = Colors.Gray;
		public int outlineSize = 5;
		public Color outlineColor = Colors.DarkGray;
		public int shadowSize = 20;
		public Color shadowColor = Colors.DarkRed;
    }
	private readonly static List<LabelSettingsData> LabelSettingsDataList = new();
	public void ReceiveCombo(float newCombo)
    {
		LabelSettingsData nextLabelSettingsData = LabelSettingsDataList.FirstOrDefault(x => x.requiredCombo >= newCombo);
		LabelSettings.FontSize = nextLabelSettingsData.fontSize;
		LabelSettings.FontColor = nextLabelSettingsData.fontColor;
		LabelSettings.OutlineSize = nextLabelSettingsData.outlineSize;
		LabelSettings.OutlineColor = nextLabelSettingsData.outlineColor;
		LabelSettings.ShadowSize = nextLabelSettingsData.shadowSize;
		LabelSettings.ShadowColor = nextLabelSettingsData.shadowColor;
    }
}
