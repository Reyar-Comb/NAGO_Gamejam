using Godot;
using System;

public partial class WontonNoodles : Cuisine
{
    public override string CuisineName => "Wonton Noodles";
    protected override string TexturePath => "res://Assets/Cuisines/WontonNoodles.png";
    public override void OnCollected()
    {
        GD.Print("Wonton Noodles collected!");
    }
}
