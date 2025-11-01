using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerUniversalState : State
{
	[Export] public int ScanFrameInterval = 5;
	private Player _player;
	private Area2D _interactArea;
	private List<PickupPoint> _nearbyPickupPoints = new();
	private PickupPoint _highlightedPickupPoint;
	private List<Customer> _nearbyCustomers = new();
	private Customer _highlightedCustomer;
	private int _frameCounter = 0;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_interactArea = Storage.GetNode<Area2D>("InteractArea");
		_interactArea.BodyEntered += OnBodyEntered;
		_interactArea.BodyExited += OnBodyExited;

		AudioManager.Instance.LoadSFX("Interact", "res://Assets/SoundFX/Interact.mp3");
	}
	private void OnBodyEntered(Node2D body)
	{
		if (body is PickupPoint pickupPoint && pickupPoint.AssignedCuisine != null
		&& !_nearbyPickupPoints.Contains(pickupPoint))
			_nearbyPickupPoints.Add(pickupPoint);
		if (body is Customer customer && customer.IsSeated && !customer.IsDelivered
		&& !customer.IsLeaving && !_nearbyCustomers.Contains(customer))
			_nearbyCustomers.Add(customer);
	}
	private void OnBodyExited(Node2D body)
	{
		if (body is PickupPoint pickupPoint)
		{
			_nearbyPickupPoints.Remove(pickupPoint);
			pickupPoint.ToggleHighlight(false);
		}
		if (body is Customer customer)
		{
			_nearbyCustomers.Remove(customer);
			customer.ToggleHighlight(false);
		}
	}
	private void RunScan()
	{
		foreach (var body in _interactArea.GetOverlappingBodies())
			OnBodyEntered(body);
	}
	protected override void FrameUpdate(double delta)
	{
		UpdatePickupPointHighlight();
		UpdateCustomerHighlight();
		_frameCounter++;
		if (_frameCounter % ScanFrameInterval == 0)
        {
			_frameCounter = 0;
			RunScan();
        }
		if (Input.IsActionJustPressed("Interact"))
		{
			if (_player.CurrentCuisine == null && _highlightedPickupPoint != null)
			{
				_player.CurrentCuisine = _highlightedPickupPoint.AssignedCuisine;
				_highlightedPickupPoint.AssignedCuisine = null;
				_nearbyPickupPoints.Remove(_highlightedPickupPoint);
				_highlightedPickupPoint.ToggleHighlight(false);

				AudioManager.Instance.PlaySFX("Interact");
			}
			else if (_player.CurrentCuisine != null && _highlightedCustomer != null)
			{
				_highlightedCustomer.ReceiveCuisine(_player.CurrentCuisine);
				_nearbyCustomers.Remove(_highlightedCustomer);
				_highlightedCustomer.ToggleHighlight(false);
				_player.CurrentCuisine = null;

				AudioManager.Instance.PlaySFX("Interact");
			}
		}
	}
	private void UpdatePickupPointHighlight()
	{
		if (_nearbyPickupPoints.Count == 0)
		{
			_highlightedPickupPoint = null;
			return;
		}
		_nearbyPickupPoints.Sort((a, b) =>
			a.GlobalPosition.DistanceTo(_player.GlobalPosition)
			.CompareTo(b.GlobalPosition.DistanceTo(_player.GlobalPosition))
		);
		if (_player.CurrentCuisine != null)
		{
			_highlightedPickupPoint = null;
			return;
		}
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
		_nearbyCustomers.Sort((a, b) =>
			a.CollisionShapePos.DistanceTo(_player.GlobalPosition)
			.CompareTo(b.CollisionShapePos.DistanceTo(_player.GlobalPosition))
		);
		if (_player.CurrentCuisine == null)
		{
			_highlightedCustomer = null;
			return;
		}
		_highlightedCustomer = _nearbyCustomers[0];
		_highlightedCustomer.ToggleHighlight(true);
		for (int i = 1; i < _nearbyCustomers.Count; i++)
			_nearbyCustomers[i].ToggleHighlight(false);
	}
}
