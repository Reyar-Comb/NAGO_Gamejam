using Godot;
using System;

public partial class InGameDisplay : CanvasLayer
{
	private Label _scoreDisplayLabel = null;
	private Label _negativeViewsDisplayLabel = null; 
	private Label _remainingTimeDisplayLabel = null;
	private ComboDisplay _comboDisplayLabel = null;
	private void InitializeNodeReferences()
	{
		_scoreDisplayLabel = GetNode<Label>("%ScoreDisplayLabel");
		_negativeViewsDisplayLabel = GetNode<Label>("%NegativeViewsDisplayLabel");
		_remainingTimeDisplayLabel = GetNode<Label>("%RemainingTimeDisplayLabel");
		_comboDisplayLabel = GetNode<ComboDisplay>("%ComboLabel");
	}
	private void ConnectSignals()
	{
		SignalBus.Instance.ScoreUpdated += OnScoreUpdated;
		SignalBus.Instance.NegativeViewsReceived += OnNegativeViewsReceived;
		SignalBus.Instance.TimeUpdated += OnTimeUpdated;
	}
	public override void _ExitTree()
	{
		SignalBus.Instance.ScoreUpdated -= OnScoreUpdated;
		SignalBus.Instance.NegativeViewsReceived -= OnNegativeViewsReceived;
		SignalBus.Instance.TimeUpdated -= OnTimeUpdated;
	}
	public override void _Ready()
	{
		InitializeNodeReferences();
		ConnectSignals();
	}
	public void OnScoreUpdated(int newScore)
	{
		_scoreDisplayLabel.Text = "分数: " + newScore.ToString();
	}
	public void OnNegativeViewsReceived(int newNegativeViews)
	{
		_negativeViewsDisplayLabel.Text = "差评: " + newNegativeViews.ToString();
	}
	public void OnTimeUpdated(float remainingTimeInSeconds)
	{
		int remainingTime = (int)remainingTimeInSeconds;
		int minutes = (int)(remainingTime / 60);
		int seconds = (int)(remainingTime % 60);
		string minutesString = minutes <= 9 ? "0" + minutes.ToString() : minutes.ToString();
		string secondsString = seconds <= 9 ? "0" + seconds.ToString() : seconds.ToString();
		_remainingTimeDisplayLabel.Text = $"{minutesString} : {secondsString}";
	}
	public void OnComboUpdated(float newCombo)
    {
		_comboDisplayLabel.ReceiveCombo(newCombo);
    }
}
