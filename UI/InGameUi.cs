using Godot;
using System;

public partial class InGameUi : CanvasLayer
{
	private Label _scoreDisplayLabel = null;
	private void InitializeNodeReferences()
	{
		_scoreDisplayLabel = GetNode<Label>("%ScoreDisplayLabel");
	}
	private void ConnectSignals()
	{
		SignalBus.Instance.ScoreUpdated += OnScoreUpdated;
	}
	public override void _Ready()
    {
		InitializeNodeReferences();
		ConnectSignals();
    }
	public override void _Process(double delta)
	{
	}
	public void OnScoreUpdated(int newScore)
	{
		_scoreDisplayLabel.Text = newScore.ToString();
	}
}
