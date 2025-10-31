using Godot;
using System;

public partial class Testastar : Node2D
{
	[Export] public TestNpc NPC;
	public override void _Ready()
	{
		NPC.MoveTo(new Vector2(-126, 94));
	}
}
