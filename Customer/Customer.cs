using Godot;
using System;

public partial class Customer : CharacterBody2D
{
	[Export] public float Speed = 100f;
	[Export] public NavigationManager NavigationManager;
	[Export] public Timer PatienceTimer;
	[Export] public Timer CuisineEatingTimer;
	[Export] public float CuisineEatingTime = 5f;
	public Cuisine DesiredCuisine;
	public bool IsSeated
	{
		get => field;
		set
		{
			if (field == value)
				return;
			field = value;
			if (field)
				OnSeated();
		}
	} = false;
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
	public Vector2 TargetChairPosition;
	private Vector2[] path = Array.Empty<Vector2>();
	private int currentStep = 0;
	private ShaderMaterial _customerShaderMaterial = null;
	private bool _isPlayerNearby = false;
	public override void _Ready()
	{
		DesiredCuisine = Cuisine.GetRandomCuisine();
		PatienceTimer.Timeout += OnPatienceTimeout;
		CuisineEatingTimer.Timeout += OnCuisineFinished;
		_customerShaderMaterial = GetNode<Sprite2D>("Sprite2D").Material as ShaderMaterial;
	}
	private void ToggleWhiteBorder(bool enable)
	{
		_customerShaderMaterial.SetShaderParameter("outline_enabled", enable);
	}
	private void OnBodyEntered(Node2D body)
	{
		_isPlayerNearby = true;
		if (body is not Player || !IsSeated || IsDelivered) return;
		ToggleWhiteBorder(true);
	}
	private void OnBodyExited(Node2D body)
	{
		_isPlayerNearby = false;
		ToggleWhiteBorder(false);
	}
	public void MoveTo(Vector2 targetPosition)
	{
		path = NavigationManager.GetPath(GlobalPosition, targetPosition);
		currentStep = 0;

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
		if (path.Length == 0 || currentStep >= path.Length)
		{
			Velocity = Vector2.Zero;
			return;
		}

		Vector2 target = path[currentStep];
		Vector2 direction = (target - GlobalPosition).Normalized();
		Velocity = direction * Speed;

		if (GlobalPosition.DistanceTo(target) < 5f)
		{
			currentStep++;

			if (currentStep >= path.Length)
			{
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
	}
	protected virtual void OnSeated() { }
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
