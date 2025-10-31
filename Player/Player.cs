using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 300.0f;
	[Export] public Sprite2D CuisineDisplaySprite;
	public Cuisine CurrentCuisine
    {
		get => field;
		set
		{
			if (value == null)
			{
				GD.PushError("Player: Receiving null Cuisine!");
				return;
			}
			if (value.CuisineTexture == null)
			{
				GD.PushError("Player: Received Cuisine has no texture!");
				return;
			}
			field = value;
			GD.Print($"Player: Updating Cuisine Display to {field.CuisineName}");
			field.OnCollected();
			CuisineDisplaySprite.Texture = field.CuisineTexture;
		}
    }
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		if (direction != Vector2.Zero)
			velocity = direction * Speed;
		else
			velocity = velocity.MoveToward(Vector2.Zero, Speed);

		Velocity = velocity;
		MoveAndSlide();
	}
}