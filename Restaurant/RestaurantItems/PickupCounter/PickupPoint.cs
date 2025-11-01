using Godot;
using System;
[GlobalClass]
public partial class PickupPoint : StaticBody2D
{
    private Sprite2D Sprite => GetNode<Sprite2D>("Sprite2D");
    private ShaderMaterial HighlightMaterial => Sprite.Material as ShaderMaterial;

    public Cuisine AssignedCuisine
    {
        get => field;
        set
        {
            if (value != null && value.CuisineTexture != null)
            {
                field = value;
                Sprite.Texture = field.CuisineTexture;
            }
            else
            {
                Visible = false;
            }
        }
    } = null;
    public override void _Ready()
    {
        AssignedCuisine = Cuisine.GetRandomCuisine();
    }
    public void ToggleHighlight(bool enabled)
    {
        HighlightMaterial.SetShaderParameter("outline_enabled", enabled);
    }
}
