using Godot;
using System;

public partial class StaminaBar : Sprite2D
{
	[Export] public float MaxStamina = 100.0f;
	[Export] public float StaminaRegenRate = 10.0f;
	[Export] public float StaminaDecreaseRate = 30.0f;
	[Export] public float StaminaRegenDelay = 1.0f;
	[Export] public float StaminaTweenDuration = 0.1f;
	[Export] public Timer StaminaRegenDelayTimer = null;
	public float Value
	{
		get => _staminaBarMaterial.GetShaderParameter("current_value").As<float>();
		set => _staminaBarMaterial.SetShaderParameter("current_value", value);
	}
	public float CurrentStamina
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
			_staminaTween.TweenMethod(Callable.From<float>((val) => SetStamina(field)), Value, field, StaminaTweenDuration);
		}
	}
	private Tween _staminaTween = null;
	private bool _canRegen = true;
	private ShaderMaterial _staminaBarMaterial = null;
	private void SetInitialShaderParameters()
	{
		_staminaBarMaterial.SetShaderParameter("max_value", MaxStamina);
		_staminaBarMaterial.SetShaderParameter("min_value", 0f);
		CurrentStamina = MaxStamina;
		_staminaBarMaterial.SetShaderParameter("current_value", CurrentStamina);
	}
	public override void _Ready()
	{
		_staminaBarMaterial = (ShaderMaterial)Material;
		SetInitialShaderParameters();
		StaminaRegenDelayTimer.Timeout += OnStaminaRegenDelayTimeout;
	}
	public override void _Process(double delta)
	{
		if (_canRegen)
			RegenStamina(delta);
		if (Input.IsActionPressed("Dash"))
			CurrentStamina -= StaminaDecreaseRate * (float)delta;
	}
	public void RegenStamina(double delta)
	{
		CurrentStamina += StaminaRegenRate * (float)delta;
	}
	private void OnStaminaRegenDelayTimeout()
	{
		_canRegen = true;
	}
	private void SetStamina(float targetStamina)
	{
		_staminaBarMaterial.SetShaderParameter("current_value", targetStamina);
	}
}
