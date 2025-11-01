using Godot;
using System;
[GlobalClass]
public partial class RoastGoose : Cuisine
{
    public override string CuisineName => "Roast Goose";
    protected override string TexturePath => "res://Assets/Cuisines/RoastGoose.png";
}
