using System.IO;
using UnityEngine;

namespace GameFoundation
{
	public static class GameSaveSystem
	{
		const string SaveName = "save.json";
		const string WebKey = "GameFoundation.Save";

		static string SavePath => Path.Combine(Application.persistentDataPath, SaveName);

		public static void Save(GameGlobals globals)
		{
			if (globals == null)
				return;

			string json = JsonUtility.ToJson(globals.CaptureSaveData(), true);
#if UNITY_WEBGL && !UNITY_EDITOR
			PlayerPrefs.SetString(WebKey, json);
			PlayerPrefs.Save();
#else
			Directory.CreateDirectory(Application.persistentDataPath);
			File.WriteAllText(SavePath, json);
#endif
		}

		public static GameSaveData Load()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			if (!PlayerPrefs.HasKey(WebKey))
				return null;
			string json = PlayerPrefs.GetString(WebKey);
#else
			if (!File.Exists(SavePath))
				return null;
			string json = File.ReadAllText(SavePath);
#endif
			return string.IsNullOrWhiteSpace(json) ? null : JsonUtility.FromJson<GameSaveData>(json);
		}

		public static bool SaveExists()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			return PlayerPrefs.HasKey(WebKey);
#else
			return File.Exists(SavePath);
#endif
		}

		public static void Reset()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			PlayerPrefs.DeleteKey(WebKey);
			PlayerPrefs.Save();
#else
			if (File.Exists(SavePath))
				File.Delete(SavePath);
#endif
		}
	}
}
