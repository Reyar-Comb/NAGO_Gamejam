using Godot;
using System;

public partial class InGameDisplay : CanvasLayer
{
	private Label _scoreDisplayLabel = null;
	private Label _negativeViewsDisplayLabel = null;
	private Label _dayDisplayLabel = null;
	private Label _remainingTimeDisplayLabel = null;
	private void InitializeNodeReferences()
	{
		_scoreDisplayLabel = GetNode<Label>("%ScoreDisplayLabel");
		_negativeViewsDisplayLabel = GetNode<Label>("%NegativeViewsDisplayLabel");
		_dayDisplayLabel = GetNode<Label>("%DayDisplayLabel");
		_remainingTimeDisplayLabel = GetNode<Label>("%RemainingTimeDisplayLabel");
	}
	private void ConnectSignals()
	{
		SignalBus.Instance.ScoreUpdated += OnScoreUpdated;
		SignalBus.Instance.NegativeViewsReceived += OnNegativeViewsReceived;
		SignalBus.Instance.DayChanged += OnDayChanged;
		SignalBus.Instance.TimeUpdated += OnTimeUpdated;
	}
	public override void _ExitTree()
	{
		SignalBus.Instance.ScoreUpdated -= OnScoreUpdated;
		SignalBus.Instance.NegativeViewsReceived -= OnNegativeViewsReceived;
		SignalBus.Instance.DayChanged -= OnDayChanged;
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
	public void OnDayChanged(int newDay)
	{
		_dayDisplayLabel.Text = "第 " + newDay.ToString() + " 天";
	}
	public void OnTimeUpdated(float remainingTimeInSeconds)
	{
		int remainingTime = (int)remainingTimeInSeconds;
		int minutes = (int)(remainingTime / 60);
		int seconds = (int)(remainingTime % 60);
		_remainingTimeDisplayLabel.Text = $"{minutes} : {seconds}";
	}
}
