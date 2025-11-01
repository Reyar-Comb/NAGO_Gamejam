using Godot;
using System;

public partial class Chair : Node2D
{
	public enum ChairType
	{
		Up,
		Left,
		Right
	}
	[Export] public ChairType Type;
	public bool IsOccupied { get; set; } = false;

	public void Occupy()
	{
		IsOccupied = true;
	}

	public void Vacate()
	{
		IsOccupied = false;
	}
}
