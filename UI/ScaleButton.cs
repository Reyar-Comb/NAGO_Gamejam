using Godot;
using System;

public partial class ScaleButton : Button
{
	[Export] public Vector2 PressedScale { get; set; } = new Vector2(0.9f, 0.9f);
	[Export] public float PressedScaleDuration { get; set; } = 0.1f;
	[Export] public Vector2 FloatScale { get; set; } = new Vector2(1.1f, 1.1f);
	[Export] public float FloatScaleDuration { get; set; } = 0.1f;
	protected virtual void ReadyBehavior() {}
	public sealed override void _Ready()
	{
		ButtonDown += OnButtonDown;
		ButtonUp += OnButtonUp;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		PivotOffset = Size / 2;
		ProcessMode = ProcessModeEnum.Always;
		ReadyBehavior();
	}
	private void OnButtonDown()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", PressedScale, PressedScaleDuration)
			.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		AudioManager.Instance.PlaySFX("ButtonClick");
	}
	private void OnButtonUp()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.One, PressedScaleDuration)
			.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
	}
	private void OnMouseEntered()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", FloatScale, FloatScaleDuration)
			.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		AudioManager.Instance.PlaySFX("ButtonHover");
	}
	private void OnMouseExited()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.One, FloatScaleDuration)
			.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
	}
}
