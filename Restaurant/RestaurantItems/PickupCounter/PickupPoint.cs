using Godot;
using System;
[GlobalClass]
public partial class PickupPoint : Sprite2D
{
    public Cuisine AssignedCuisine
    {
        get => field;
        set
        {
            if (field != null && field.CuisineTexture != null)
            {
                field = value;
                Texture = field.CuisineTexture;
            }
            else
            {
                GD.PushError("PickupPoint: Assigned Cuisine is null or has no texture!");
                Visible = false;
            }
        }
    } = null;
}
