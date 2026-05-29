using System;
using System.Reflection;
using UnityEngine;

namespace GameFoundation
{
	[DefaultExecutionOrder(-950)]
	public class GamePowerQuestBridge : MonoBehaviour
	{
		public static GamePowerQuestBridge Instance { get; private set; }

		Type powerQuestType;
		object powerQuestInstance;

		public bool PowerQuestAvailable { get; private set; }

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
			DetectPowerQuest();
		}

		public void DetectPowerQuest()
		{
			powerQuestType = Type.GetType("PowerTools.Quest.PowerQuest, Assembly-CSharp");
			PowerQuestAvailable = powerQuestType != null;

			if (PowerQuestAvailable)
			{
				PropertyInfo getProperty = powerQuestType.GetProperty("Get", BindingFlags.Public | BindingFlags.Static);
				powerQuestInstance = getProperty != null ? getProperty.GetValue(null, null) : null;
			}
		}

		public bool RequestRoomChange(string roomName)
		{
			if (!PowerQuestAvailable)
			{
				GameSceneManager.Instance?.ChangeScene(roomName, GameTransitionType.FadeToBlack, false);
				return true;
			}

			// Integration point: once PowerQuest rooms are authored, resolve the room object by
			// name and invoke ChangeRoom/ChangeRoomBG through reflection here.
			Debug.Log($"PowerQuest detected. Room change requested: {roomName}. Add project-specific room lookup in GamePowerQuestBridge.");
			GameSceneManager.Instance?.ChangeScene(roomName, GameTransitionType.FadeToBlack, false);
			return true;
		}
	}
}
