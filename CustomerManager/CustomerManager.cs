using Godot;
using System;
using System.Collections.Generic;

public partial class CustomerManager : Node
{
    [Export] public Godot.Collections.Array<PackedScene> CustomerList = new();
    [Export] public Godot.Collections.Array<Chair> ChairList = new();
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

        Chair TargetChair = null;

        var randomChairIndex = GD.Randi() % AvailableChairList.Count;
        TargetChair = AvailableChairList[(int)randomChairIndex];
        AvailableChairList.Remove(TargetChair);

        // Instantiate a random customer
        var randomIndex = GD.Randi() % CustomerList.Count;
        var customerScene = CustomerList[(int)randomIndex];
        var customerInstance = customerScene.Instantiate<Node2D>();

        // Position the customer at the chair's location
        customerInstance.GlobalPosition = new Vector2(0, 0);

        customerInstance.Set("TargetChairPosition", TargetChair.GlobalPosition);

        // Add the customer to the scene tree
        GetTree().CurrentScene.AddChild(customerInstance);

        GD.Print("CustomerManager: Spawned a new customer at chair position.");
    }
}
