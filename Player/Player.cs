using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
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
}
