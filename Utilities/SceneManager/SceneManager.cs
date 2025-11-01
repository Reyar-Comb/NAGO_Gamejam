using Godot;
using System;
using System.Threading.Tasks;
public partial class SceneManager : Node
{
    [Export] public PackedScene RestaurantScene;
	public static SceneManager Instance { get; private set; }
	public TransitionLayer Transition => GetNode<TransitionLayer>("TransitionLayer");
	public async void ChangeScene(PackedScene scene)
    {
		await Transition.FadeIn(0.5f);
        GetTree().ChangeSceneToPacked(scene);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		await Transition.FadeOut(0.5f);
	}
	public async void ChangeScenePath(string scene)
	{
		await Transition.FadeIn(0.5f);
		GetTree().ChangeSceneToFile(scene);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		await Transition.FadeOut(0.5f);
	}
    public async void ReloadRestaurantScene()
    {
        var currentScene = GetTree().CurrentScene;
        ChangeScene(RestaurantScene);
    }
	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
			ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			QueueFree();
		}
	}
}
