using Godot;
using System;

public partial class RiceRolls : Cuisine
{
    public override string CuisineName => "Rice Rolls";
    protected override string TexturePath => "res://Assets/Cuisines/RiceRolls.png";
    public override void OnCollected()
    {
        base.OnCollected();
    }

}
