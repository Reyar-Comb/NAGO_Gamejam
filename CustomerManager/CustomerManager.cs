using Godot;
using System;
using System.Collections.Generic;

public partial class CustomerManager : Node
{
    [Export] public Godot.Collections.Array<PackedScene> CustomerList = new();
    [Export] public Godot.Collections.Array<Chair> ChairList = new();
    [Export] public Godot.Collections.Array<Vector2> SpawnPositions = new();
    public Godot.Collections.Array<Chair> AvailableChairList = new();
    public static CustomerManager Instance { get; private set; }
    public override void _Ready()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;

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

        customerInstance.TargetChairPosition = targetChair.GlobalPosition;

        // Add the customer to the scene tree
        GetTree().CurrentScene.AddChild(customerInstance);

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
