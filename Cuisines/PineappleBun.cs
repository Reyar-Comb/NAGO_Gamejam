using Godot;
using System;

public partial class PineappleBun : Cuisine
{
    public override string CuisineName => "Pineapple Bun";
    protected override string TexturePath => "res://Assets/Cuisines/PineappleBun.png";
    public override void OnCollected()
    {
        base.OnCollected();
    }

}
