using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Probability : RefCounted
{
	public bool IsEmpty => _probableActions.Count == 0;
	private List<Tuple<float, Action>> _probableActions = new();
	private RandomNumberGenerator _rng = new();
	public Probability Register(float weight, Action action)
	{
		_probableActions.Add(new(weight, action));
		return this;
	}
	public void Run()
	{
		if (_probableActions.Count == 0)
		{
			GD.PushWarning("Can't run the Probability because there's no probable action.");
			return;
		}
		_rng.Randomize();
		float[] weights = _probableActions.Select(x => (float)x.Item1).ToArray();
		int resultIndex = (int)_rng.RandWeighted(weights);
		_probableActions[resultIndex].Item2?.Invoke();
	}
	public static void Run(params Tuple<float, Action>[] probableActions)
	{
		using Probability probability = new();

		foreach (var tuple in probableActions)
			probability.Register(tuple.Item1, tuple.Item2);
		
		probability.Run();
	}
	public static void RunIfElse(float firstProbability, Action first, Action second)
	{
		firstProbability = Mathf.Clamp(firstProbability, 0f, 1f);
		Run(
			new Tuple<float, Action>(firstProbability, first),
			new Tuple<float, Action>(1.0f - firstProbability, second)
		);
	}
	public static void RunSingle(float probability, Action action) => RunIfElse(probability, action, () => { });
	public static void RunUniform(params Action[] actions)
	{
		if (actions.Length == 0)
		{
			GD.PushWarning("RunUniform: No actions provided.");
			return;
		}
		float uniformProbability = 1.0f / actions.Length;
		var probableActions = actions.Select(a => new Tuple<float, Action>(uniformProbability, a)).ToArray();
		Run(probableActions);
	}
	public static T RunUniformChoose<T>(params T[] items)
	{
		if (items.Length == 0)
		{
			GD.PushWarning("RunUniformChoose: No items provided.");
			return default;
		}
		T chosenItem = default;
		RunUniform(items.Select(item => new Action(() => chosenItem = item)).ToArray());
		return chosenItem;
	}
	public static T RunWeightedChoose<T>(T[] items, float[] weights)
	{
		if (items.Length == 0)
		{
			GD.PushWarning("RunWeightedChoose: No items provided.");
			return default;
		}
		if (items.Length != weights.Length)
		{
			GD.PushError("RunWeightedChoose: Items and weights length mismatch.");
			return default;
		}
		T chosenItem = default;
		var probableActions = items.Zip(weights, (item, weight) => new Tuple<float, Action>(weight, new Action(() => chosenItem = item))).ToArray();
		Run(probableActions);
		return chosenItem;
	}
}
