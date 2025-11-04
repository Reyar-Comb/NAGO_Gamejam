using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerUniversalState : State
{
	private Player _player;
	private Area2D _interactArea;
	private Area2D _takeCuisineArea;
	private List<PickupPoint> _nearbyPickupPoints = new();
	private PickupPoint _highlightedPickupPoint;
	private List<Customer> _nearbyCustomers = new();
	private Customer _highlightedCustomer = null;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_interactArea = Storage.GetNode<Area2D>("InteractArea");
		_takeCuisineArea = Storage.GetNode<Area2D>("TakeCuisineArea");
		_interactArea.BodyEntered += OnInteractAreaBodyEntered;
		_interactArea.BodyExited += OnInteractAreaBodyExited;
		_takeCuisineArea.BodyEntered += OnTakeCuisineAreaBodyEntered;
		_takeCuisineArea.BodyExited += OnTakeCuisineAreaBodyExited;

		AudioManager.Instance.LoadSFX("Interact", "res://Assets/SoundFX/Interact.mp3");
	}
	private void OnInteractAreaBodyEntered(Node2D body)
	{
		if (body is Customer customer && customer.IsSeated && !customer.IsDelivered
		&& !customer.IsLeaving && !_nearbyCustomers.Contains(customer))
			_nearbyCustomers.Add(customer);
	}
	private void OnInteractAreaBodyExited(Node2D body)
	{
		if (body is Customer customer)
		{
			_nearbyCustomers.Remove(customer);
			customer.ToggleHighlight(false);
		}
	}
	private void OnTakeCuisineAreaBodyEntered(Node2D body)
	{
		if (body is PickupPoint pickupPoint && pickupPoint.AssignedCuisine != null
		&& !_nearbyPickupPoints.Contains(pickupPoint) && _highlightedCustomer == null)
			_nearbyPickupPoints.Add(pickupPoint);
	}
	private void OnTakeCuisineAreaBodyExited(Node2D body)
	{
		if (body is PickupPoint pickupPoint)
		{
			_nearbyPickupPoints.Remove(pickupPoint);
			pickupPoint.ToggleHighlight(false);
		}
	}
	protected override void Exit()
	{
		_nearbyCustomers.Clear();
		_nearbyPickupPoints.Clear();
		_highlightedCustomer = null;
		_highlightedPickupPoint = null;
	}
	private void RunScan()
	{
		foreach (var body in _takeCuisineArea.GetOverlappingBodies())
			OnTakeCuisineAreaBodyEntered(body);
		foreach (var body in _interactArea.GetOverlappingBodies())
			OnInteractAreaBodyEntered(body);
	}
	protected override void FrameUpdate(double delta)
	{
		RunScan();
		UpdatePickupPointHighlight();
		UpdateCustomerHighlight();

		if (Input.IsActionJustPressed("Interact"))
		{
			OnPickupPointInteracted(_highlightedPickupPoint);
			OnCustomerInteracted(_highlightedCustomer);
		}
	}
	private void OnPickupPointInteracted(PickupPoint pickupPoint)
	{
		if (pickupPoint is null || pickupPoint?.AssignedCuisine is null) return;
		_player.CurrentCuisine = pickupPoint.AssignedCuisine;
		GD.Print("** Player picked up cuisine: " + _player.CurrentCuisine.CuisineName);
		pickupPoint.AssignedCuisine = null;
		_highlightedPickupPoint = null;
		_nearbyPickupPoints.Remove(pickupPoint);
		pickupPoint.ToggleHighlight(false);
	}
	private void OnCustomerInteracted(Customer customer)
	{
		if (_player.CurrentCuisine is null || customer is null) return;
		customer.ReceiveCuisine(_player.CurrentCuisine);
		_player.CurrentCuisine = null;
		_nearbyCustomers.Remove(customer);
		customer.ToggleHighlight(false);
		_highlightedCustomer = null;
		AudioManager.Instance.PlaySFX("Interact");
	}
	private void UpdatePickupPointHighlight()
	{
		if (_nearbyPickupPoints.Count == 0)
		{
			_highlightedPickupPoint = null;
			return;
		}
		if (_highlightedCustomer != null && _player.CurrentCuisine != null)
		{
			_highlightedPickupPoint = null;
			return;
		}
		_nearbyPickupPoints.Sort((a, b) =>
			a.GlobalPosition.DistanceTo(_player.GlobalPosition)
			.CompareTo(b.GlobalPosition.DistanceTo(_player.GlobalPosition))
		);

		_highlightedPickupPoint = _nearbyPickupPoints[0];
		_highlightedPickupPoint.ToggleHighlight(true);
		for (int i = 1; i < _nearbyPickupPoints.Count; i++)
			_nearbyPickupPoints[i].ToggleHighlight(false);
	}
	private void UpdateCustomerHighlight()
	{
		if (_nearbyCustomers.Count == 0)
		{
			_highlightedCustomer = null;
			return;
		}
		if (_player.CurrentCuisine == null)
		{
			_highlightedCustomer = null;
			return;
		}
		_nearbyCustomers.Sort((a, b) =>
			a.CollisionShapePos.DistanceTo(_player.GlobalPosition)
			.CompareTo(b.CollisionShapePos.DistanceTo(_player.GlobalPosition))
		);

		_highlightedCustomer = _nearbyCustomers[0];
		_highlightedCustomer.ToggleHighlight(true);
		for (int i = 1; i < _nearbyCustomers.Count; i++)
			_nearbyCustomers[i].ToggleHighlight(false);
	}
}
