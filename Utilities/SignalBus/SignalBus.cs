using Godot;
using System;

public partial class SignalBus : Node
{
    [Signal] public delegate void ScoreUpdatedEventHandler(int newScore);
    [Signal] public delegate void NegativeViewsReceivedEventHandler(int newNegativeViews);
    [Signal] public delegate void TimeUpdatedEventHandler(float timePassed);
    [Signal] public delegate void ComboUpdatedEventHandler(float newCombo);
    [Signal] public delegate void InGameMenuSettingsToggledEventHandler();
    [Signal] public delegate void CustomerSatisfiedEventHandler();
    [Signal] public delegate void DialogueStartedEventHandler();
    [Signal] public delegate void DialogueEndedEventHandler();
    [Signal] public delegate void GameStartEventHandler();
    [Signal] public delegate void ComboReached10EventHandler();
    [Signal] public delegate void ComboBoostEndedEventHandler();
    [Signal] public delegate void MinutePassedEventHandler();
    public static SignalBus Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }
}
