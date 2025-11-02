using Godot;
using System;

public partial class ComboVBoxContainer : VBoxContainer
{
	[Export] public Vector2 TargetScale = new Vector2(1.1f, 1.1f);
	[Export] public float ScaleChangeDuration = 0.2f;	
	public override void _Ready()
	{
		SignalBus.Instance.CustomerSatisfied += OnCustomerSatisfied;
		PivotOffset = Size / 2;
	}
    public override void _ExitTree()
    {
        SignalBus.Instance.CustomerSatisfied -= OnCustomerSatisfied;
    }
	private void OnCustomerSatisfied()
    {
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", TargetScale, ScaleChangeDuration);
		tween.TweenProperty(this, "scale", Vector2.One, ScaleChangeDuration);
    }
}
