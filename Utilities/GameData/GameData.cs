using Godot;
using System;

public partial class GameData : Node
{
    public static GameData Instance { get; private set; }
    public int Score = 0;
    public override void _Ready()
    {
        Instance = this;
    }
}
