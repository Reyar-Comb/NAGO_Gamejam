using Godot;
using System;

public partial class GameData : Node
{
    public static GameData Instance { get; private set; }
    public int Score = 0;
    public int Level = 1;
    public int NegativeViews = 0;
    public override void _Ready()
    {
        Instance = this;
    }
}
