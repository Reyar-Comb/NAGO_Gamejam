using Godot;
using System;
[GlobalClass]
public partial class PickupPoint : StaticBody2D
{
    [Export] public string CuisineName = "";
    [Export] public float RespawnTime = 2.0f;
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
                GetTree().CreateTimer(RespawnTime).Timeout += () =>
                {
                    AssignedCuisine = Cuisine.GetCuisineByName(CuisineName);
                    Visible = true;
                };
            }
        }
    } = null;
    public override void _Ready()
    {
        AssignedCuisine = Cuisine.GetCuisineByName(CuisineName);
    }
    public void ToggleHighlight(bool enabled)
    {
        HighlightMaterial.SetShaderParameter("outline_enabled", enabled);
    }
}
