using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;


public partial class AudioManager : Node
{
	public static AudioManager Instance { get; private set; }

	public AudioStreamPlayer BGMPlayer;
	public List<AudioStreamPlayer> SFXPlayers = new List<AudioStreamPlayer>();

	public Dictionary<string, AudioStream> BGMDict = new Dictionary<string, AudioStream>();
	public Dictionary<string, AudioStream> SFXDict = new Dictionary<string, AudioStream>();

	public float DefaultBGMVolume = -6f;
	public float DefaultAllSFXVolume = -6f;
	public float DefaultSFXVolume = 0f;
	public float loopStart = 0f;
	public float loopEnd = 0f;
	public bool isLooping = false;

	public override void _Ready()
	{
		// 单例模式
		if (Instance == null)
		{
			Instance = this;
			// 确保切换场景时不被销毁
			ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			QueueFree();
		}
		BGMPlayer = new AudioStreamPlayer();
		BGMPlayer.Bus = "BGM";
		AddChild(BGMPlayer);
		setBGMVolume(DefaultBGMVolume);

		for (int i = 0; i < 10; i++)
		{
			var SFXPlayer = new AudioStreamPlayer();
			SFXPlayer.Bus = "SFX";
			SFXPlayers.Add(SFXPlayer);
			AddChild(SFXPlayer);
			SFXPlayer.Autoplay = false;
		}
		setAllSFXVolume(DefaultAllSFXVolume);
	}
	public void LoadBGM(string name, string path)
	{
		var stream = GD.Load<AudioStream>(path);
		if (stream != null)
		{
			BGMDict[name] = stream;
		}
	}

	public void LoadSFX(string name, string path)
	{
		var stream = GD.Load<AudioStream>(path);
		if (stream != null)
		{
			SFXDict[name] = stream;
		}
	}

	public async void PlayBGM(string name, float startLoop = 0f, float endLoop = 0f, float startTime = 0f, float fadeTime = 0f)
	{

		if (BGMDict.ContainsKey(name))
		{
			BGMPlayer.Stream = BGMDict[name];

			if (fadeTime > 0f)
			{
				setBGMVolume(-80f);
				BGMPlayer.Play();
				BGMPlayer.Seek(startTime);
				await FadeVolume(name, -80f, DefaultBGMVolume, fadeTime, "BGM");
			}
			else
			{
				setBGMVolume(DefaultBGMVolume);
				BGMPlayer.Play();
				BGMPlayer.Seek(startTime);
			}
			isLooping = (endLoop > startLoop) && (endLoop > 0f);
			loopStart = startLoop;
			loopEnd = endLoop;
		}
	}

	public async void PlaySFX(string name, float fadeTime = 0f)
	{
		
		if (SFXDict.ContainsKey(name))
		{
			foreach (var sfx in SFXPlayers)
			{

				if (!sfx.Playing)
				{
					sfx.Stream = SFXDict[name];
					if (fadeTime > 0f)
					{
						sfx.VolumeDb = -80f;
						sfx.Play();
						await FadeVolume(name, -80f, DefaultSFXVolume, fadeTime, "SFX");
					}
					else
					{
						setSFXVolume(name, DefaultSFXVolume);
						sfx.Play();
					}
					break;
				}
			}
		}
	}

	public async void StopBGM(float fadeTime = 0f)
	{
		if (fadeTime > 0f)
		{
			await FadeVolume("bgm doesnt need name", DefaultBGMVolume, -80f, fadeTime, "BGM");
		}
		BGMPlayer.Stop();
		isLooping = false;
	}


	public async void StopSFX(string name, float fadeTime = 0f)
	{
		
		if (fadeTime > 0f)
		{
			await FadeVolume(name, DefaultSFXVolume, -80f, fadeTime, "SFX");
		}
		foreach (var sfx in SFXPlayers)
		{
			if (sfx.Stream == SFXDict.GetValueOrDefault(name) && sfx.Playing)
			{
				sfx.Stop();
				return;
			}
		}
	}

	public async System.Threading.Tasks.Task FadeVolume(string name, float fromDb, float toDb, float duration, string bus = "BGM")
	{
		float fromLinear = Mathf.DbToLinear(fromDb);
		float toLinear = Mathf.DbToLinear(toDb);
		float elapsed = 0f;
		while (elapsed < duration)
		{
			float t = elapsed / duration;
			float currentLinear = Mathf.Lerp(fromLinear, toLinear, t);
			float currentDb = Mathf.LinearToDb(currentLinear);
			if (bus == "BGM")

				setBGMVolume(currentDb);
			else
				setSFXVolume(name, currentDb);
			await ToSignal(GetTree(), "process_frame");
			elapsed += (float)GetProcessDeltaTime();
		}
		if (bus == "BGM")
			setBGMVolume(toDb);
		else
			setSFXVolume(name, toDb);
	}


	public override void _Process(double delta)
	{
		if (isLooping && BGMPlayer.Playing)
		{
			if (BGMPlayer.GetPlaybackPosition() >= loopEnd)
			{
				BGMPlayer.Seek(loopStart);
			}
		}
		

	}



	public void setBGMVolume(float volume)
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("BGM"), volume);
	}

	public void setAllSFXVolume(float volume)
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), volume);
	}

	public void setSFXVolume(string name, float volume)
	{
		foreach (var sfx in SFXPlayers)
		{
			if (sfx.Stream == SFXDict.GetValueOrDefault(name))
			{
				sfx.VolumeDb = volume;
			}
		}
	}
}
