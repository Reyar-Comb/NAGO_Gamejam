using Godot;
using System;

public partial class Restaurant : Node2D
{
	[Export] public Timer CuisineSpawnTimer;
	[Export] public Player Waiter;
	public override void _Ready()
	{
		CuisineSpawnTimer.Timeout += OnCuisineSpawnTimerTimeout;
	}

	public override void _Process(double delta)
	{
	}
	private void OnCuisineSpawnTimerTimeout()
	{
		Cuisine cuisine = Cuisine.GetRandomCuisine();
		if (cuisine != null)
			GD.Print($"Spawn Cuisine {cuisine.CuisineName}");
		Waiter.CurrentCuisine = cuisine;
	}
}
