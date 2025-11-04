using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class CustomerManager : Node
{
	[Export] public Godot.Collections.Array<PackedScene> CustomerList = new();
	[Export] public Godot.Collections.Array<Chair> ChairList = new();
	[Export] public Godot.Collections.Array<Marker2D> SpawnPositions = new();
	[Export] public float InitialSpawnInterval = 10f;
	[Export] public float MinSpawnInterval = 2f;
	[Export] public float SpawnTimeDecrementPerMinute = 1f;
	[Export] public float RefreshInterval = 30f;
	[Export] public Timer CustomerSpawnTimer;
	[Export] public Timer SpawnRefreshTimer;
	public Godot.Collections.Array<Chair> AvailableChairList = new();
	public float SpawnInterval => Mathf.Max(
		InitialSpawnInterval - GameData.Instance.TimePassed / 60 * SpawnTimeDecrementPerMinute,
		MinSpawnInterval);

	public override async void _Ready()
	{
		await ToSignal(GetTree().CurrentScene, SignalName.Ready);
		await ToSignal(SignalBus.Instance, SignalBus.SignalName.GameStart);
		InitializeAvailableChairs();
		CustomerSpawnTimer.WaitTime = SpawnInterval;
		CustomerSpawnTimer.Start(SpawnInterval);
		CustomerSpawnTimer.Timeout += SpawnCustomer;
		SpawnRefreshTimer.WaitTime = RefreshInterval;
		SpawnRefreshTimer.Timeout += () => CustomerSpawnTimer.WaitTime = SpawnInterval;
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
	// private bool CheckCollision(Vector2 targetChairPosition)
	// {
	// 	Node2D container = GetNode<Node2D>("%CustomerContainer");
	// 	for (int i = 0; i < container.GetChildCount(); i++)
	// 	{
	// 		Customer customer = container.GetChild<Customer>(i);
	// 		if (customer.TargetChairPosition.DistanceTo(targetChairPosition) < 10f && customer.IsSeated && !customer.IsLeaving)
	// 			return true;
	// 	}
	// 	return false;
	// }
	public void SpawnCustomer()
	{
		if (CustomerList.Count == 0 || ChairList.Count == 0 || AvailableChairList.Count == 0)
		{
			GD.PrintErr("CustomerManager: No customers or chairs available to spawn.");
			return;
		}

		Chair targetChair = null;

		targetChair = GetRandomElement(AvailableChairList);
		// GD.Print("** Chair occupied status before assignment: " + targetChair.IsOccupied.ToString());
		// if (targetChair.IsOccupied || CheckCollision(targetChair.GetNode<Marker2D>("Marker2D").GlobalPosition))
		// {
		// 	GD.PrintErr("FUCK: CustomerManager: Selected chair is already occupied or colliding. Aborting spawn.");
		// 	GD.PrintErr("FUCK: Available chairs count: " + AvailableChairList.Count + ", Total chairs count: " + ChairList.Count);
		// 	for (int i = 0; i < AvailableChairList.Count; i++)
		// 	{
		// 		GD.PrintErr("FUCK: Available chair " + i + " is " + (AvailableChairList[i].IsOccupied ? "occupied" : "available"));
		// 	}
		// 	return;
		// }
		AvailableChairList.Remove(targetChair);
		targetChair.IsOccupied = true;
		// Instantiate a random customer
		var customerScene = GetRandomElement(CustomerList);
		var customerInstance = customerScene.Instantiate<Customer>();

		// Position the customer at the chair's location
		customerInstance.GlobalPosition = GetRandomElement(SpawnPositions).GlobalPosition;
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
