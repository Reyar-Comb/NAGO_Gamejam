using Godot;
using System;
using System.Collections;

public partial class Trail : Sprite2D
{
    public float Lifetime = 0.5f;
    public override void _Ready()
    {
        Modulate = new(1, 1, 1, 0.5f);
        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0f, Lifetime).SetTrans(Tween.TransitionType.Linear);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
