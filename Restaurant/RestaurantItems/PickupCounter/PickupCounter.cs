using Godot;
using System;

public partial class PickupCounter : Node2D
{
	[Export] public Node2D PickupPointContainer;
	[Export] public Sprite2D ChefSprite;
	[Export] public float ChefPopupChance = 0.3f;
	[Export] public Marker2D UnseenMarker;
	[Export] public Marker2D PopupMarker;
	private Tween _chefTween;
	public override void _Ready()
	{
		foreach (Node2D child in PickupPointContainer.GetChildren())
			if (child is PickupPoint pickupPoint)
				pickupPoint.Connect(PickupPoint.SignalName.CuisineSpawned, Callable.From(OnCuisineSpawned));
	}
	private void OnCuisineSpawned()
	{
		if (GD.Randf() <= ChefPopupChance)
		{
			_chefTween = CreateTween();
			_chefTween.TweenProperty(ChefSprite, "global_position", PopupMarker.GlobalPosition, 0.3f)
				.SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);
			_chefTween.TweenInterval(1.0f);
			_chefTween.TweenProperty(ChefSprite, "global_position", UnseenMarker.GlobalPosition, 0.3f)
				.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
		}
	}
}