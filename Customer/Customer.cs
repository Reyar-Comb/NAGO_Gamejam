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
	[Export] public Panel OrderDisplayPanel;
	[Export] public float MinPatienceTime = 10f;
	[Export] public float MaxPatienceTime = 60f;
	[Export] public float PatienceTimeRandomness = 5f;
	[Export] public PackedScene[] RubbishScenes;
	public float CurrentBasePatienceTime => (float)Mathf.Max(
		MaxPatienceTime - GameData.Instance.TimePassed / 60 * 3,
		MinPatienceTime);
	public float PatienceTime => (float)Mathf.Clamp(
		CurrentBasePatienceTime + GD.RandRange(-PatienceTimeRandomness, PatienceTimeRandomness),
		MinPatienceTime,
		MaxPatienceTime);
	public Vector2 CollisionShapePos => GetNode<CollisionShape2D>("CollisionShape2D").GlobalPosition;
	public bool CanReceiveCuisine = false;
	public Chair.ChairType TargetChairType;
	public Cuisine DesiredCuisine { get; private set; }
	private Vector2 _panelOriginalPosition;
	public bool IsSeated
	{
		get => field;
		set
		{
			if (field == value)
				return;
			field = value;
			if (field)
			{
				GetTree().CreateTimer(0.5f).Timeout += () => CanReceiveCuisine = true;
				OrderDisplayPanel.Visible = true;
				GetNode<TextureRect>("%CuisineIcon").Texture = DesiredCuisine.CuisineTexture;
				_patienceTime = PatienceTime;
				PatienceTimer.Start(_patienceTime);
				_panelOriginalPosition = OrderDisplayPanel.Position;
				GetNode<Node2D>("Offset").Position = DecideOrderPanelPosition();
			}
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
				OrderDisplayPanel.Visible = false;
				PatienceTimer.Stop();
			}
		}
	} = false;
	public bool IsLeaving = false;
	public Vector2 TargetChairPosition;
	private Vector2[] path = Array.Empty<Vector2>();
	private int _currentStep = 0;
	private ShaderMaterial CustomerShaderMaterial => GetNode<AnimatedSprite2D>("AnimatedSprite2D").Material as ShaderMaterial;
	private bool _isPlayerNearby = false;
	private float _timeElapsed = 0f;
	private float _leftAndRightXOffset = -300f;
	private float _patienceTime = 0f;
	private Vector2 DecideOrderPanelPosition()
	{
		return TargetChairType switch
		{
			Chair.ChairType.Up => new Vector2(-400, -700),
			Chair.ChairType.Left => new Vector2(61 + _leftAndRightXOffset, -1070),
			Chair.ChairType.Right => new Vector2(-261 + _leftAndRightXOffset, -1135),
			_ => new Vector2(0, -150),
		};
	}
	private void DecideThrowRubbish()
	{
		float throwChance = Mathf.Clamp(0.1f + GameData.Instance.TimePassed / 60 * 0.025f, 0.1f, 0.3f);
		bool willThrow = GD.Randf() < throwChance;
		if (!willThrow) return;
		PackedScene rubbishScene = Probability.RunUniformChoose(RubbishScenes);
		Node2D instance = rubbishScene.Instantiate<Node2D>();
		if (TargetChairType == Chair.ChairType.Up)
		{
			instance.GlobalPosition = TargetChairPosition + Vector2.Up * 150f + Vector2.Right * GD.RandRange(150, 300) * Probability.RunUniformChoose([-1f, 1f]);
		}
		else if (TargetChairType == Chair.ChairType.Left)
		{
			float angle = (float)GD.RandRange(-Mathf.Pi / 2, 0);
			instance.GlobalPosition = TargetChairPosition + Vector2.Left.Rotated(angle) * GD.RandRange(150, 500);
		}
		else
		{
			float angle = (float)GD.RandRange(0, Mathf.Pi / 2);
			instance.GlobalPosition = TargetChairPosition + Vector2.Right.Rotated(angle) * GD.RandRange(150, 500);
		}
		GetTree().CurrentScene.AddChild(instance);
	}
	public override async void _Ready()
	{
		DesiredCuisine = Cuisine.GetRandomCuisine();
		PatienceTimer.Timeout += OnPatienceTimeout;
		CuisineEatingTimer.Timeout += OnCuisineFinished;
		Visible = false;
		AudioManager.Instance.LoadSFX("Satisfied", "res://Assets/SoundFX/Satisfied2.mp3");
		AudioManager.Instance.LoadSFX("Wrong", "res://Assets/SoundFX/Wrong3.mp3");
		AudioManager.Instance.LoadSFX("Coin", "res://Assets/SoundFX/Coin.mp3");
		await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);
		Visible = true;
		MoveTo(TargetChairPosition);
		DesiredCuisine = Cuisine.GetRandomCuisine();
	}
	private void GetAngry()
	{
		GameData.Instance.Combo = 0;
		GameData.Instance.NegativeViews++;
		Leave();
	}
	public async void ReceiveCuisine(Cuisine cuisine)
	{
		if (cuisine.CuisineName == DesiredCuisine.CuisineName)
		{
			AudioManager.Instance.PlaySFX("Satisfied");
			IsDelivered = true;
			await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.CustomerSatisfied);
			cuisine.OnDelivered(1 + (float)PatienceTimer.TimeLeft / _patienceTime);
			DecideThrowRubbish();
			GameData.Instance.Combo++;
			Leave();
		}
		else
		{
			AudioManager.Instance.PlaySFX("Wrong");
			GetAngry();
			GD.Print("** Highlighted customer wants " + DesiredCuisine.CuisineName + " but received " + cuisine.CuisineName);
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
		Vector2 panelOffset = Vector2.Zero;
		if (IsSeated && !IsDelivered)
		{
			_timeElapsed += (float)delta;
			panelOffset.Y = Mathf.Sin(_timeElapsed * 3f) * 5f;
		}
		OrderDisplayPanel.Position = _panelOriginalPosition + panelOffset;
		GetNode<Label>("%RemainingTimeLabel").Text = Mathf.Floor(PatienceTimer.TimeLeft).ToString();
		if (_isPlayerNearby && Input.IsActionJustPressed("Interact") && !IsDelivered && IsOrdered)
		{
			Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
			GD.Print($"Customer has received cuisine: {player.CurrentCuisine.CuisineName}");
			IsDelivered = true;
		}
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
			AudioManager.Instance.PlaySFX("Coin");

		OrderDisplayPanel.Visible = false;
		EmitSignal(SignalName.CustomerLeft);
		GetTree().CreateTimer(20f).Timeout += () =>
		{
			if (IsInstanceValid(this))
				QueueFree();
		};
	}
	protected virtual void OnOrdered() { }
	protected virtual void OnCuisineDelivered() { }
	protected virtual void OnCuisineFinished() { }
	protected void OnPatienceTimeout()
	{
		GD.Print("顾客的耐心用尽了！");
		GetAngry();
	}
}
