using Godot;
using System;

public partial class GameData : Node
{
    public static GameData Instance { get; private set; }
    public int Score
    {
        get => field;
        set
        {
            field = value;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.ScoreUpdated, field);
        }
    } = 0;
    public int Day
    {
        get => field;
        set
        {
            field = value;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.DayChanged, field);
        }
    } = 1;
    public int NegativeViews
    {
        get => field;
        set
        {
            field = value;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.NegativeViewsReceived, field);
        }
    } = 0;
    public float RemainingTimeInSeconds
    {
        get => field;
        set
        {
            field = value;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.TimeUpdated, field);
        }
    } = 180;
    public override async void _Ready()
    {
        Instance = this;
        await ToSignal(GetTree().CurrentScene, Node.SignalName.Ready);
        UpdateInGameDisplay();
        GetTree().SceneChanged += async () =>
        {
            await ToSignal(GetTree().CurrentScene, Node.SignalName.Ready);
            RemainingTimeInSeconds = 180;
            NegativeViews = 0;
            Day++;
            UpdateInGameDisplay();
        };
    }
    public void ResetGameData()
    {
        Score = 0;
        Day = 1;
        NegativeViews = 0;
        RemainingTimeInSeconds = 600;
        UpdateInGameDisplay();
    }
    private void UpdateInGameDisplay()
    {
        Score = Score;
        NegativeViews = NegativeViews;
        Day = Day;
        RemainingTimeInSeconds = RemainingTimeInSeconds;
    }
    public override void _Process(double delta)
    {
        RemainingTimeInSeconds -= (float)delta;
    }
}
