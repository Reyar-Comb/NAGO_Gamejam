using Godot;
using System;
[GlobalClass]
public partial class Meal : Cuisine
{
    public override string CuisineName => "Meal";
    protected override string TexturePath => "res://icon.svg";
    
    public override void OnCollected()
    {
        GD.Print("Meal Collected!");
        GameData.Instance.Score += 10;
    }
}
