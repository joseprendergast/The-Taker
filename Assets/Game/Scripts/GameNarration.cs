using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFoundation
{
	[Serializable]
	public class GameNarrationClip
	{
		public string lineId;
		public AudioClip clip;
	}

	[DefaultExecutionOrder(-700)]
	[RequireComponent(typeof(AudioSource))]
	public class GameNarration : MonoBehaviour
	{
		public static GameNarration Instance { get; private set; }

		[SerializeField] GameNarrationProviderType provider = GameNarrationProviderType.PreGeneratedClip;
		[SerializeField] string narratorSpeakerName = "Narrator";
		[SerializeField] string externalTextToSpeechProxyUrl;
		[SerializeField] List<GameNarrationClip> narrationClips = new List<GameNarrationClip>();

		AudioSource audioSource;
		Coroutine activeFallback;

		public bool IsPlaying => audioSource != null && audioSource.isPlaying;
		public GameNarrationProviderType Provider => provider;
		public string ExternalTextToSpeechProxyUrl => externalTextToSpeechProxyUrl;

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
			audioSource = GetComponent<AudioSource>();
			audioSource.playOnAwake = false;
			ApplySettings();
		}

		public void SetEnabled(bool enabled)
		{
			if (GameGlobals.Instance != null)
				GameGlobals.Instance.Settings.narrationEnabled = enabled;

			if (!enabled)
				Stop();
		}

		public void SetVolume(float volume)
		{
			if (GameGlobals.Instance != null)
				GameGlobals.Instance.Settings.narrationVolume = Mathf.Clamp01(volume);

			ApplySettings();
		}

		public void SetVoiceId(string voiceId)
		{
			if (GameGlobals.Instance != null)
				GameGlobals.Instance.Settings.narratorVoiceId = voiceId;
		}

		public void SetVoiceDirection(string voiceDirection)
		{
			if (GameGlobals.Instance != null)
				GameGlobals.Instance.Settings.narratorVoiceDirection = voiceDirection;
		}

		public void Narrate(string lineId, string text, float fallbackHoldSeconds = 3.5f)
		{
			if (!CanNarrate(text))
				return;

			Stop();
			ApplySettings();

			if (provider == GameNarrationProviderType.PreGeneratedClip && TryPlayClip(lineId))
				return;

			if (provider == GameNarrationProviderType.ExternalTextToSpeechProxy)
				Debug.LogWarning("GameNarration external TTS proxy is configured as an integration point only. Do not place OpenAI API keys in a Unity WebGL client.");

			GameDialogue.Instance?.Say(narratorSpeakerName, text, string.Empty, false, fallbackHoldSeconds);
			activeFallback = StartCoroutine(FallbackSubtitleHold(fallbackHoldSeconds));
		}

		public void Narrate(SubtitleLineData line)
		{
			if (line == null)
				return;

			Narrate(line.flagOnFirstPlay, line.text, line.holdSeconds);
		}

		public void Stop()
		{
			if (activeFallback != null)
			{
				StopCoroutine(activeFallback);
				activeFallback = null;
			}

			if (audioSource != null)
				audioSource.Stop();
		}

		bool CanNarrate(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return false;

			GameSettingsData settings = GameGlobals.Instance != null ? GameGlobals.Instance.Settings : new GameSettingsData();
			return settings.narrationEnabled && !settings.muted;
		}

		bool TryPlayClip(string lineId)
		{
			if (string.IsNullOrWhiteSpace(lineId))
				return false;

			GameNarrationClip narrationClip = narrationClips.Find(item => item != null && item.lineId == lineId);
			if (narrationClip == null || narrationClip.clip == null)
				return false;

			audioSource.clip = narrationClip.clip;
			audioSource.Play();
			return true;
		}

		void ApplySettings()
		{
			GameSettingsData settings = GameGlobals.Instance != null ? GameGlobals.Instance.Settings : new GameSettingsData();
			if (audioSource != null)
				audioSource.volume = settings.muted ? 0f : settings.narrationVolume * settings.masterVolume;
		}

		IEnumerator FallbackSubtitleHold(float holdSeconds)
		{
			yield return new WaitForSeconds(Mathf.Max(0.25f, holdSeconds));
			activeFallback = null;
		}
	}
}
