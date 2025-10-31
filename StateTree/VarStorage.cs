using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class VarStorage : Node
{
	// 初始变量存储,在Ready时复制到变量存储
	[Export] private Dictionary<string, Variant> _initialVariantStorage = new();
	private Dictionary<string, Variant> _variantStorage = new();
	// 存储节点的引用
	[Export] private Dictionary<string, Node> _nodeStorage = new();
	public override void _Ready()
	{
		foreach (var pair in _initialVariantStorage)
			_variantStorage.Add(pair.Key, pair.Value);
	}
	// 注册变量和节点
	public void RegisterVariant<[MustBeVariant] T>(string varName, Variant variant) => _variantStorage.Add(varName, variant);
	public void RegisterNode<T>(string varName, T node) where T : Node => _nodeStorage.Add(varName, node);
	// 获取变量与节点
	public T GetVariant<[MustBeVariant] T>(string varName) => _variantStorage[varName].As<T>();
	public T GetNode<T>(string varName) where T : Node => (T)_nodeStorage[varName];
	// 设置变量
	public void SetVariant(string varName, Variant variant) => _variantStorage[varName] = variant;
	// 移除变量与节点
	public void RemoveVariant(string varName) => _variantStorage.Remove(varName);
	public void RemoveNode(string varName) => _nodeStorage.Remove(varName);
}
