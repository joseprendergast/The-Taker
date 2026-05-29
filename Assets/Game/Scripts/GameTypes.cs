using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFoundation
{
	public enum ContextualActionType
	{
		Context,
		Inspect,
		Talk,
		UseItem,
		Move
	}

	public enum GameTransitionType
	{
		HardCut,
		FadeToBlack,
		Dissolve
	}

	public enum GameNarrationProviderType
	{
		None,
		PreGeneratedClip,
		ExternalTextToSpeechProxy
	}

	[Serializable]
	public class GamePaletteData
	{
		public Color shadows = new Color(0.02f, 0.025f, 0.04f, 1f);
		public Color midtones = new Color(0.23f, 0.25f, 0.31f, 1f);
		public Color highlights = new Color(0.92f, 0.88f, 0.74f, 1f);
		public Color dangerAccent = new Color(0.65f, 0.18f, 0.18f, 1f);
		public Color uiText = new Color(0.95f, 0.93f, 0.86f, 1f);
		public Color uiBorder = new Color(0.78f, 0.66f, 0.38f, 1f);
		public Color disabledText = new Color(0.48f, 0.48f, 0.52f, 1f);
	}

	[Serializable]
	public class SubtitleLineData
	{
		public string speaker;
		[TextArea(2, 5)] public string text;
		public string flagOnFirstPlay;
		public bool playOnce;
		public float holdSeconds = 2.5f;
	}

	[Serializable]
	public class InventoryItemData
	{
		public string id;
		public string displayName;
		[TextArea(2, 4)] public string inspectLine;
		public bool hidden;
		public bool disabled;
	}

	[Serializable]
	public class GameSceneDefinition
	{
		public string sceneId;
		public string displayName;
		[TextArea(2, 5)] public string description;
		public string backgroundArtReference;
		public List<string> charactersPresent = new List<string>();
		public List<string> ambientEffects = new List<string>();
		public List<SubtitleLineData> narrativeBeats = new List<SubtitleLineData>();
		public List<string> requiredFlags = new List<string>();
		public string completionFlag;
		public bool debugJumpEnabled = true;
	}

	[Serializable]
	public class GameSettingsData
	{
		public bool muted;
		[Range(0f, 1f)] public float masterVolume = 1f;
		[Range(0f, 1f)] public float musicVolume = 0.8f;
		[Range(0f, 1f)] public float ambienceVolume = 0.8f;
		[Range(0f, 1f)] public float sfxVolume = 0.9f;
		public bool typewriterSubtitles = true;
		[Range(0.75f, 2f)] public float subtitleSize = 1f;
		public bool narrationEnabled = true;
		[Range(0f, 1f)] public float narrationVolume = 0.9f;
		public string narratorVoiceId = "onyx";
		public string narratorVoiceDirection = "Low, restrained British male narrator. Professional, grave, intimate, and cinematic.";
		public bool crtFilter;
		public bool integerScaling = true;
	}

	[Serializable]
	public class GameSaveData
	{
		public int currentChapter;
		public string currentScene;
		public List<string> visitedScenes = new List<string>();
		public List<string> storyFlags = new List<string>();
		public List<string> puzzleFlags = new List<string>();
		public List<string> collectedItems = new List<string>();
		public List<string> dialogueFlags = new List<string>();
		public GameSettingsData settings = new GameSettingsData();
		public long savedAtUnixSeconds;
	}
}
