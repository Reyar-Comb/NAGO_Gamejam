using Godot;
using System;

public partial class SignalBus : Node
{
    [Signal] public delegate void ScoreUpdatedEventHandler(int newScore);
    public static SignalBus Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }
}
