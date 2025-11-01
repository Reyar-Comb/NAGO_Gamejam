using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerUniversalState : State
{
	private Player _player;
	private Area2D _pickupArea;
	private List<PickupPoint> _nearbyPickupPoints = new();
	private PickupPoint _highlightedPickupPoint;
	protected override void ReadyBehavior()
	{
		_player = Storage.GetNode<Player>("Player");
		_pickupArea = Storage.GetNode<Area2D>("PickupArea");
		_pickupArea.BodyEntered += OnBodyEntered;
		_pickupArea.BodyExited += OnBodyExited;
	}
	private void OnBodyEntered(Node2D body)
	{
		if (body is PickupPoint pickupPoint && pickupPoint.AssignedCuisine != null)
		{
			_nearbyPickupPoints.Add(pickupPoint);
		}
	}
	private void OnBodyExited(Node2D body)
	{
		if (body is PickupPoint pickupPoint)
		{
			_nearbyPickupPoints.Remove(pickupPoint);
			pickupPoint.ToggleHighlight(false);
		}
	}
	protected override void FrameUpdate(double delta)
	{
		UpdateHighlight();
		if (Input.IsActionJustPressed("Interact") && _highlightedPickupPoint != null)
		{
			_player.CurrentCuisine = _highlightedPickupPoint.AssignedCuisine;
			_highlightedPickupPoint.AssignedCuisine = null;
			_nearbyPickupPoints.Remove(_highlightedPickupPoint);
			_highlightedPickupPoint.ToggleHighlight(false);
		}
	}
	private void UpdateHighlight()
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
		_highlightedPickupPoint = _nearbyPickupPoints[0];
		_highlightedPickupPoint.ToggleHighlight(true);
		for (int i = 1; i < _nearbyPickupPoints.Count; i++)
			_nearbyPickupPoints[i].ToggleHighlight(false);
	}
}
