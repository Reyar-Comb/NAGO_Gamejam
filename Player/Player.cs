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
			field = value;
			if (value == null)
				GD.PushWarning("Player: Receiving null Cuisine! Is this intended?");
			
			if (value?.CuisineTexture == null)
			{
				GD.PushWarning("Player: Received Cuisine has no texture! Is this intended?");
				CuisineDisplaySprite.Texture = null;
			}
			if (string.IsNullOrEmpty(value?.CuisineName))
				GD.PushWarning("Player: Received Cuisine has no name! Is this intended?");
			
			field?.OnCollected();
			CuisineDisplaySprite.Texture = field?.CuisineTexture;
		}
	}
}
