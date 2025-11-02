using Godot;
using System;
using System.Threading.Tasks;
public partial class StartMenu : Control
{
	[Export] public TextureRect Title;
	
	
	private Vector2 TitlePos;
	private Tween _titleTween;

	private Tween _buttonTween;
	public override async void _Ready()
	{
		TitlePos = Title.GlobalPosition;

		AudioManager.Instance.LoadBGM("BGM", "res://Assets/SoundFX/bgm.mp3");
		TitleAnimation();
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		AudioManager.Instance.PlayBGM("BGM");
	}


	public void TitleAnimation()
	{
		if (_titleTween != null)
		{
			_titleTween.Kill();  // 停止旧动画
		}
		_titleTween = CreateTween();
		_titleTween.SetTrans(Tween.TransitionType.Sine);
		_titleTween.SetLoops(-1);
		_titleTween.TweenProperty(Title, "global_position", TitlePos - new Vector2(0, -20f), 1f).SetEase(Tween.EaseType.InOut);
		_titleTween.TweenProperty(Title, "global_position", TitlePos - new Vector2(0, 20f), 1f).SetEase(Tween.EaseType.InOut);

	}

	
}
