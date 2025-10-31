using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class State : Node
{
	[Signal] public delegate void TransitEventHandler(string targetStateName);
	[Export] public VarStorage Storage = null;
	public State PreviousState = null;
	public State Parent = null;

	private bool _isActive = false;
	public override sealed void _Ready()
	{
		ReadyBehavior();
	}
	public override sealed void _EnterTree()
	{
		EnterTreeBehavior();
		if (this is not StateTree) Parent = GetParent<State>();
	}
	public override sealed void _PhysicsProcess(double delta)
	{
		if (!_isActive) return;
		PhysicsUpdate(delta);
	}
	public override sealed void _Process(double delta)
	{
		if (!_isActive) return;
		FrameUpdate(delta);
	}
	// 转移至对应状态
	public void AskTransit(string targetStateName) => EmitSignal(SignalName.Transit, targetStateName);
	public void EnterState()
	{
		_isActive = true;
		Enter();
	}
	// 重载以实现进入状态时的行为
	protected virtual void Enter() { }
	public void ExitState()
	{
		Exit();
		_isActive = false;
	}
	// 重载以实现退出状态时的行为
	protected virtual void Exit() { }
	// 重载以实现物理帧更新行为
	protected virtual void PhysicsUpdate(double delta) { }
	// 重载以实现帧更新行为
	protected virtual void FrameUpdate(double delta) { }
	// 重载以实现节点准备行为
	protected virtual void ReadyBehavior() { }
	// 重载以实现节点进入场景树行为
	protected virtual void EnterTreeBehavior() { }
}
