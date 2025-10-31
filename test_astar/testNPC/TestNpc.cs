using Godot;
using System;

public partial class TestNpc : CharacterBody2D
{
	[Export] public float Speed = 100f;
	[Export] public NavigationManager NavigationManager;

	private Vector2[] path = Array.Empty<Vector2>();
	private int currentStep = 0;

	public void MoveTo(Vector2 targetPosition)
	{
		path = NavigationManager.GetPath(GlobalPosition, targetPosition);
		currentStep = 0;

		if (path.Length > 0)
		{
			GD.Print("Path found with " + path.Length + " steps.");
		}
		else
		{
			GD.Print("No path found.");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (path.Length == 0 || currentStep >= path.Length)
		{
			Velocity = Vector2.Zero;
			return;
		}

		Vector2 target = path[currentStep];
		Vector2 direction = (target - GlobalPosition).Normalized();
		Velocity = direction * Speed;

		if (GlobalPosition.DistanceTo(target) < 5f)
		{
			currentStep++;

			if (currentStep >= path.Length)
			{
				GD.Print("顾客到达餐桌！");
				path = Array.Empty<Vector2>();
			}
		}
		MoveAndSlide();
	}
}
