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
    public int NegativeViews
    {
        get => field;
        set
        {
            field = value;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.NegativeViewsReceived, field);
        }
    } = 0;
    public float TimePassed
    {
        get => field;
        set
        {
            field = value;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.TimeUpdated, field);
        }
    } = 0;
    public int Combo
    {
        get => field;
        set
        {
            field = value;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.ComboUpdated, field);
        }
    } = 0;
    public override async void _Ready()
    {
        Instance = this;
        await ToSignal(GetTree().CurrentScene, Node.SignalName.Ready);
        UpdateInGameDisplay();
    }
    public void ResetGameData()
    {
        Score = 0;
        NegativeViews = 0;
        TimePassed = 0;
        Combo = 0;
        UpdateInGameDisplay();
    }
    private void UpdateInGameDisplay()
    {
        Score = Score;
        NegativeViews = NegativeViews;
        TimePassed = TimePassed;
        Combo = Combo;
    }
    public override void _Process(double delta)
    {
        TimePassed += (float)delta;
    }
}
