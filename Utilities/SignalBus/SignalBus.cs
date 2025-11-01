using Godot;
using System;

public partial class SignalBus : Node
{
    [Signal] public delegate void ScoreUpdatedEventHandler(int newScore);
    [Signal] public delegate void NegativeViewsReceivedEventHandler(int newNegativeViews);
    [Signal] public delegate void DayChangedEventHandler(int newDay);
    [Signal] public delegate void TimeUpdatedEventHandler(float remainingTimeInSeconds);
    [Signal] public delegate void CustomerSatisfiedEventHandler();
    public static SignalBus Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }
}
