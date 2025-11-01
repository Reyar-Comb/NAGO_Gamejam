using Godot;
using System;
using System.Threading.Tasks;

public partial class Customer : CharacterBody2D
{
	[Signal] public delegate void CustomerLeftEventHandler();
	[Export] public float Speed = 300f;
	[Export] public NavigationManager NavigationManager;
	[Export] public Timer PatienceTimer;
	[Export] public Timer CuisineEatingTimer;
	[Export] public float CuisineEatingTime = 5f;
	[Export] public AnimatedSprite2D AnimatedSprite;
	public Vector2 CollisionShapePos => GetNode<CollisionShape2D>("CollisionShape2D").GlobalPosition;
	public Chair.ChairType TargetChairType;
	public Cuisine DesiredCuisine { get; private set; }
	public bool IsSeated = false;
	public bool IsOrdered
	{
		get => field;
		set
		{
			if (field == value)
				return;
			field = value;
			if (field)
				OnOrdered();
		}
	} = false;
	public bool IsDelivered
	{
		get => field;
		set
		{
			if (field == value)
				return;
			field = value;
			if (field)
			{
				OnCuisineDelivered();
				StartCuisineEatingTimer(CuisineEatingTime);
			}
		}
	} = false;
	public bool IsLeaving = false;
	public Vector2 TargetChairPosition;
	private Vector2[] path = Array.Empty<Vector2>();
	private int _currentStep = 0;
	private ShaderMaterial CustomerShaderMaterial => GetNode<AnimatedSprite2D>("AnimatedSprite2D").Material as ShaderMaterial;
	private bool _isPlayerNearby = false;
	public override async void _Ready()
	{
		DesiredCuisine = Cuisine.GetRandomCuisine();
		PatienceTimer.Timeout += OnPatienceTimeout;
		CuisineEatingTimer.Timeout += OnCuisineFinished;

		AudioManager.Instance.LoadSFX("Satisfied", "res://Assets/SoundFX/Satisfied2.mp3");
		AudioManager.Instance.LoadSFX("Wrong", "res://Assets/SoundFX/Wrong3.mp3");
		AudioManager.Instance.LoadSFX("Coin", "res://Assets/SoundFX/Coin.mp3");
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		MoveTo(TargetChairPosition);
		DesiredCuisine = Cuisine.GetRandomCuisine();
	}
	public async void ReceiveCuisine(Cuisine cuisine)
	{
		if (cuisine.CuisineName == DesiredCuisine.CuisineName)
		{
			GD.Print("顾客收到了正确的菜肴: " + cuisine.CuisineName);
			AudioManager.Instance.PlaySFX("Satisfied");
			IsDelivered = true;
			await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
			Leave();
		}
		else
		{
			Leave();
			AudioManager.Instance.PlaySFX("Wrong");
			GD.Print("顾客收到了错误的菜肴: " + cuisine.CuisineName + "，期望的是: " + DesiredCuisine.CuisineName);
		}
	}
	public void ToggleHighlight(bool enable)
	{
		CustomerShaderMaterial.SetShaderParameter("outline_enabled", enable);
	}
	public void MoveTo(Vector2 targetPosition)
	{
		path = NavigationManager.GetPath(GlobalPosition, targetPosition);
		_currentStep = 0;

		if (path.Length > 0)
		{
			GD.Print("Path found with " + path.Length + " steps.");
		}
		else
		{
			GD.PushError("No path found.");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (path.Length == 0 || _currentStep >= path.Length)
		{
			Velocity = Vector2.Zero;
			return;
		}

		Vector2 target = path[_currentStep];
		Vector2 direction = (target - GlobalPosition).Normalized();
		Velocity = direction * Speed;

		if (GlobalPosition.DistanceTo(target) < 5f)
		{
			_currentStep++;

			if (_currentStep >= path.Length)
			{
				IsSeated = true;
				GD.Print("顾客到达餐桌！");
				path = Array.Empty<Vector2>();
			}
		}
		MoveAndSlide();
	}
	public override void _Process(double delta)
	{
		if (_isPlayerNearby && Input.IsActionJustPressed("Interact") && !IsDelivered && IsOrdered)
		{
			Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
			GD.Print($"Customer has received cuisine: {player.CurrentCuisine.CuisineName}");
			IsDelivered = true;
		}
	}
	public void StartPatienceTimer(float time)
	{
		PatienceTimer.Start(time);
	}
	public void StartCuisineEatingTimer(float time)
	{
		CuisineEatingTimer.Start(time);
	}
	public void Leave()
	{
		GD.Print("顾客离开餐厅。");
		IsLeaving = true;
		if (IsDelivered)
		{
			AudioManager.Instance.PlaySFX("Coin");
		}
		
		EmitSignal(SignalName.CustomerLeft);
	}
	protected virtual void OnOrdered() { }
	protected virtual void OnCuisineDelivered() { }
	protected virtual void OnCuisineFinished() { }
	protected virtual void OnPatienceRunOut() { }
	protected void OnPatienceTimeout()
	{
		GD.Print("顾客的耐心用尽了！");
		OnPatienceRunOut();
		Leave();
	}
}
