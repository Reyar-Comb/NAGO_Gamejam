using Godot;
using System;

public partial class GameData : Node
{
    public bool IsGameStarted = false;
    public static GameData Instance { get; private set; }
    public const int MaxNegativeViewsAllowed = 10;
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
            if ((int)field % 60 == 0)
            {
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.MinutePassed);
            }
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
        await ToSignal(SignalBus.Instance, SignalBus.SignalName.GameStart);
        IsGameStarted = true;
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
        if (IsGameStarted) TimePassed += (float)delta;
    }
}
