using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class CustomerManager : Node
{
	[Export] public Godot.Collections.Array<PackedScene> CustomerList = new();
	[Export] public Godot.Collections.Array<Chair> ChairList = new();
	[Export] public Godot.Collections.Array<Vector2> SpawnPositions = new();
	[Export] public float InitialSpawnInterval = 10f;
	[Export] public float MinSpawnInterval = 2f;
	[Export] public Timer CustomerSpawnTimer;
	public Godot.Collections.Array<Chair> AvailableChairList = new();
	public float SpawnInterval => Mathf.Max(
		InitialSpawnInterval - GameData.Instance.TimePassed / 60 * 0.5f,
		MinSpawnInterval);
	
	public override async void _Ready()
	{
		await ToSignal(GetTree().CurrentScene, SignalName.Ready);
		InitializeAvailableChairs();
		CustomerSpawnTimer.WaitTime = SpawnInterval;
		CustomerSpawnTimer.Start(SpawnInterval);
		CustomerSpawnTimer.Timeout += SpawnCustomer;
	}
	
	public void InitializeAvailableChairs()
	{
		if (ChairList.Count == 0)
		{
			CallDeferred("InitializeAvailableChairs");
			return;
		}
		AvailableChairList.Clear();
		foreach (var chair in ChairList)
		{
			if (!chair.IsOccupied)
			{
				AvailableChairList.Add(chair);
			}
		}
	}

	public void SpawnCustomer()
	{
		CustomerSpawnTimer.Start(SpawnInterval);
		if (CustomerList.Count == 0 || ChairList.Count == 0)
		{
			GD.PrintErr("CustomerManager: No customers or chairs available to spawn.");
			return;
		}

		Chair targetChair = null;

		targetChair = GetRandomElement(AvailableChairList);
		AvailableChairList.Remove(targetChair);

		// Instantiate a random customer
		var customerScene = GetRandomElement(CustomerList);
		var customerInstance = customerScene.Instantiate<Customer>();

		// Position the customer at the chair's location
		customerInstance.GlobalPosition = GetRandomElement(SpawnPositions);
		customerInstance.TargetChairType = targetChair.Type;
		GD.Print("CustomerManager: Assigned TargetChairType " + targetChair.Type.ToString());
		customerInstance.TargetChairPosition = targetChair.GetNode<Marker2D>("Marker2D").GlobalPosition;
		customerInstance.YSortEnabled = true;
		// Add the customer to the scene tree
		GetTree().CurrentScene.GetNode<Node2D>("CustomerContainer").AddChild(customerInstance);

		customerInstance.CustomerLeft += () =>
		{
			targetChair.IsOccupied = false;
			AvailableChairList.Add(targetChair);
			GD.Print("CustomerManager: Customer has left. Chair is now available.");
		};
		GD.Print("CustomerManager: Spawned a new customer at chair position.");
	}
	private T GetRandomElement<[MustBeVariant] T>(Godot.Collections.Array<T> array)
	{
		if (array.Count == 0)
		{
			GD.PushError("CustomerManager: Attempted to get random element from an empty array.");
			return default;
		}
		var randomIndex = GD.Randi() % array.Count;
		return array[(int)randomIndex];
	}
}
