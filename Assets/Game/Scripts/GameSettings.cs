using UnityEngine;

namespace GameFoundation
{
	[DefaultExecutionOrder(-750)]
	public class GameSettings : MonoBehaviour
	{
		public static GameSettings Instance { get; private set; }

		public bool AudioUnlocked { get; private set; }

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
			Apply();
		}

		void Update()
		{
			if (!AudioUnlocked && (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.touchCount > 0))
				UnlockAudio();
		}

		public void UnlockAudio()
		{
			AudioUnlocked = true;
			AudioListener.pause = false;
			Apply();
		}

		public void SetMuted(bool muted)
		{
			GameGlobals.Instance.Settings.muted = muted;
			Apply();
		}

		public void SetMasterVolume(float value)
		{
			GameGlobals.Instance.Settings.masterVolume = Mathf.Clamp01(value);
			Apply();
		}

		public void SetMusicVolume(float value) => GameGlobals.Instance.Settings.musicVolume = Mathf.Clamp01(value);
		public void SetAmbienceVolume(float value) => GameGlobals.Instance.Settings.ambienceVolume = Mathf.Clamp01(value);
		public void SetSfxVolume(float value) => GameGlobals.Instance.Settings.sfxVolume = Mathf.Clamp01(value);
		public void SetSubtitleSize(float value) => GameGlobals.Instance.Settings.subtitleSize = Mathf.Clamp(value, 0.75f, 2f);
		public void SetNarrationEnabled(bool enabled) => GameNarration.Instance?.SetEnabled(enabled);
		public void SetNarrationVolume(float value) => GameNarration.Instance?.SetVolume(value);
		public void SetNarratorVoiceId(string voiceId) => GameNarration.Instance?.SetVoiceId(voiceId);
		public void SetTypewriter(bool enabled) => GameGlobals.Instance.Settings.typewriterSubtitles = enabled;
		public void SetCrtFilter(bool enabled) => GameGlobals.Instance.Settings.crtFilter = enabled;
		public void SetIntegerScaling(bool enabled) => GameGlobals.Instance.Settings.integerScaling = enabled;

		public void Apply()
		{
			GameSettingsData settings = GameGlobals.Instance != null ? GameGlobals.Instance.Settings : new GameSettingsData();
			AudioListener.volume = settings.muted ? 0f : settings.masterVolume;
		}
	}
}
