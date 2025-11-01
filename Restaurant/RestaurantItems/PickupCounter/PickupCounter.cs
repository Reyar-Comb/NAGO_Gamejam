using Godot;
using System;

public partial class PickupCounter : Node2D
{
	[Export] public Node2D PickupPointContainer;
	[Export] public float ChefPopupChance = 0.3f;
	public override void _Ready()
	{
		foreach (Node2D child in PickupPointContainer.GetChildren())
		{
			if (child is PickupPoint pickupPoint)
			{
				pickupPoint.Connect(PickupPoint.SignalName.CuisineSpawned, Callable.From(OnCuisineSpawned));
			}
		}
	}
	private void OnCuisineSpawned()
	{
	}
}