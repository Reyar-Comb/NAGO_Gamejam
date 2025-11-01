using Godot;
using System;

public partial class MilkTea : Cuisine
{
    public override string CuisineName => "Milk Tea";
    protected override string TexturePath => "res://Assets/Cuisines/MilkTea.png";
    public override void OnCollected()
    {
        GD.Print("Milk Tea collected!");
    }
}
