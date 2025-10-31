using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class StateTree : State
{
	// 填写初始状态名称,将沿路径进入该状态
	[Export] public string InitialStateName = "";
	protected Dictionary<string, State> _stateTree = new();
	public State CurrentState = null;

	protected override void ReadyBehavior()
	{
		foreach (State childState in GetChildren())
			ProcessChild(childState);
		if (string.IsNullOrEmpty(InitialStateName))
		{
			GD.PushError("InitialStateName is not set in StateTree.");
			return;
		}
		SetupInitialChain();
	}
	protected void ProcessChild(State state)
	{
		state.Transit += (targetStateName)
			=> CallDeferred(MethodName.ChangeState, targetStateName);

		_stateTree.Add(state.Name, state);

		foreach (State childState in state.GetChildren())
			ProcessChild(childState);
	}
	protected void ChangeState(string targetStateName)
	{
		List<State> currentPath = GetPathToRoot(CurrentState);
		State previousState = CurrentState;
		CurrentState = _stateTree[targetStateName];
		CurrentState.PreviousState = previousState;
		List<State> nextPath = GetPathToRoot(CurrentState);
		nextPath.Reverse();
		CutIntersect(ref currentPath, ref nextPath);
		foreach (State state in currentPath)
			state?.ExitState();
		foreach (State state in nextPath)
			state?.EnterState();
	}
	protected List<State> GetPathToRoot(State state)
	{
		List<State> path = new();
		while (state != null)
		{
			path.Add(state);
			state = state.Parent;
		}
		return path;
	}
	protected void CutIntersect(ref List<State> states1, ref List<State> states2)
	{
		List<State> commonStates = states1.Intersect(states2).ToList();
		states1 = states1.Except(commonStates).ToList();
		states2 = states2.Except(commonStates).ToList();
	}
	protected void SetupInitialChain()
	{
		
		Stack<State> stack = new();
		State initialState = _stateTree[InitialStateName];
		CurrentState = initialState;


		while (initialState != null)
		{
			stack.Push(initialState);
			initialState = initialState.Parent;
		}
		
		while (stack.Count > 0)
		{
			stack.Pop().EnterState();
		}
	}
}
