using Godot;
using System;

public partial class PlayerIdleState : State
{
    private Player _player;
    protected override void ReadyBehavior()
    {
        _player = Storage.GetNode<Player>("Player");
    }
    protected override void Enter()
    {
        GD.Print("Player has entered Idle State.");
    }

    protected override void FrameUpdate(double delta)
    {
        if (!Mathf.IsZeroApprox(_player.Velocity.X))
        {
            AskTransit("Walk");
        }
    }
}
