using Godot;
using System;

public partial class StaminaBar : ProgressBar
{
	[Export] public float MaxStamina = 100.0f;
	[Export] public float StaminaRegenRate = 10.0f;
	[Export] public float StaminaRegenDelay = 1.0f;
	[Export] public Timer StaminaRegenDelayTimer = null;
	private float CurrentStamina
	{
		get => field;
		set
		{
			if (field > value)
			{
				_canRegen = false;
				StaminaRegenDelayTimer.Start(StaminaRegenDelay);
			}
			field = Mathf.Clamp(value, 0f, MaxStamina);
			_staminaTween?.Kill();
			_staminaTween = CreateTween();
			_staminaTween.TweenProperty(this, "value", field, 0.2f).SetEase(Tween.EaseType.Out);
		}
	}
	private Tween _staminaTween = null;
	private bool _canRegen = true;
	public override void _Ready()
	{
		CurrentStamina = MaxStamina;
		Value = CurrentStamina;
		StaminaRegenDelayTimer.Timeout += OnStaminaRegenDelayTimeout;
	}
	public override void _Process(double delta)
	{
		if (_canRegen)
			RegenStamina(delta);
		if (Input.IsActionJustPressed("Dash"))
			CurrentStamina -= 20.0f;
	}
	public void RegenStamina(double delta)
	{
		CurrentStamina += StaminaRegenRate * (float)delta;
	}
	private void OnStaminaRegenDelayTimeout()
	{
		_canRegen = true;
	}
}
