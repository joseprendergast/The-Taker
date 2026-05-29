using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFoundation
{
	[DefaultExecutionOrder(-1000)]
	public class GameGlobals : MonoBehaviour
	{
		public static GameGlobals Instance { get; private set; }

		[SerializeField] int currentChapter;
		[SerializeField] string currentScene = "RoomTitle";
		[SerializeField] List<string> visitedScenes = new List<string>();
		[SerializeField] List<string> storyFlags = new List<string>();
		[SerializeField] List<string> puzzleFlags = new List<string>();
		[SerializeField] List<string> dialogueFlags = new List<string>();
		[SerializeField] List<string> collectedItems = new List<string>();
		[SerializeField] GameSettingsData settings = new GameSettingsData();

		public event Action<string, bool> FlagChanged;
		public event Action InventoryChanged;

		public int CurrentChapter { get => currentChapter; set => currentChapter = value; }
		public string CurrentScene { get => currentScene; set => currentScene = value; }
		public IReadOnlyList<string> VisitedScenes => visitedScenes;
		public IReadOnlyList<string> StoryFlags => storyFlags;
		public IReadOnlyList<string> PuzzleFlags => puzzleFlags;
		public IReadOnlyList<string> DialogueFlags => dialogueFlags;
		public IReadOnlyList<string> CollectedItems => collectedItems;
		public GameSettingsData Settings => settings;

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
			SetFlag("framework.initialized");
		}

		public bool HasFlag(string flag)
		{
			return storyFlags.Contains(flag) || puzzleFlags.Contains(flag) || dialogueFlags.Contains(flag);
		}

		public void SetFlag(string flag, bool enabled = true)
		{
			SetFlagInList(storyFlags, flag, enabled);
		}

		public void SetPuzzleFlag(string flag, bool enabled = true)
		{
			SetFlagInList(puzzleFlags, flag, enabled);
		}

		public void SetDialogueFlag(string flag, bool enabled = true)
		{
			SetFlagInList(dialogueFlags, flag, enabled);
		}

		void SetFlagInList(List<string> list, string flag, bool enabled)
		{
			if (string.IsNullOrWhiteSpace(flag))
				return;

			bool changed = enabled ? AddUnique(list, flag) : list.Remove(flag);
			if (changed)
				FlagChanged?.Invoke(flag, enabled);
		}

		public void VisitScene(string sceneId)
		{
			if (string.IsNullOrWhiteSpace(sceneId))
				return;

			currentScene = sceneId;
			AddUnique(visitedScenes, sceneId);
		}

		public bool HasVisited(string sceneId)
		{
			return visitedScenes.Contains(sceneId);
		}

		public void CollectItem(string itemId)
		{
			if (AddUnique(collectedItems, itemId))
				InventoryChanged?.Invoke();
		}

		public bool HasItem(string itemId)
		{
			return collectedItems.Contains(itemId);
		}

		public GameSaveData CaptureSaveData()
		{
			return new GameSaveData
			{
				currentChapter = currentChapter,
				currentScene = currentScene,
				visitedScenes = new List<string>(visitedScenes),
				storyFlags = new List<string>(storyFlags),
				puzzleFlags = new List<string>(puzzleFlags),
				dialogueFlags = new List<string>(dialogueFlags),
				collectedItems = new List<string>(collectedItems),
				settings = settings,
				savedAtUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
			};
		}

		public void Restore(GameSaveData data)
		{
			if (data == null)
				return;

			currentChapter = data.currentChapter;
			currentScene = data.currentScene;
			visitedScenes = data.visitedScenes ?? new List<string>();
			storyFlags = data.storyFlags ?? new List<string>();
			puzzleFlags = data.puzzleFlags ?? new List<string>();
			dialogueFlags = data.dialogueFlags ?? new List<string>();
			collectedItems = data.collectedItems ?? new List<string>();
			settings = data.settings ?? new GameSettingsData();
			InventoryChanged?.Invoke();
		}

		public void ResetProgress()
		{
			currentChapter = 0;
			currentScene = "RoomTitle";
			visitedScenes.Clear();
			storyFlags.Clear();
			puzzleFlags.Clear();
			dialogueFlags.Clear();
			collectedItems.Clear();
			SetFlag("framework.reset");
			InventoryChanged?.Invoke();
		}

		static bool AddUnique(List<string> list, string value)
		{
			if (string.IsNullOrWhiteSpace(value) || list.Contains(value))
				return false;

			list.Add(value);
			return true;
		}
	}
}
