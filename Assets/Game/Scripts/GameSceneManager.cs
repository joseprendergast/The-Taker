using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFoundation
{
	[DefaultExecutionOrder(-900)]
	public class GameSceneManager : MonoBehaviour
	{
		public static GameSceneManager Instance { get; private set; }

		[SerializeField] string newGameScene = "RoomPrototypeA";
		[SerializeField] GameTransitionType defaultTransition = GameTransitionType.FadeToBlack;
		[SerializeField] bool routeThroughPowerQuestBridge = true;

		public bool IsChangingScene { get; private set; }

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public void NewGame()
		{
			GameGlobals.Instance.ResetProgress();
			GameSaveSystem.Reset();
			ChangeScene(newGameScene, defaultTransition);
		}

		public bool Continue()
		{
			GameSaveData data = GameSaveSystem.Load();
			if (data == null)
				return false;

			GameGlobals.Instance.Restore(data);
			ChangeScene(data.currentScene, defaultTransition);
			return true;
		}

		public void ChangeScene(string sceneId)
		{
			ChangeScene(sceneId, defaultTransition);
		}

		public void ChangeScene(string sceneId, GameTransitionType transition, bool allowBridge = true)
		{
			if (string.IsNullOrWhiteSpace(sceneId) || IsChangingScene)
				return;

			if (allowBridge && routeThroughPowerQuestBridge && GamePowerQuestBridge.Instance != null && GamePowerQuestBridge.Instance.PowerQuestAvailable)
			{
				if (GamePowerQuestBridge.Instance.RequestRoomChange(sceneId))
					return;
			}

			StartCoroutine(ChangeSceneRoutine(sceneId, transition));
		}

		IEnumerator ChangeSceneRoutine(string sceneId, GameTransitionType transition)
		{
			IsChangingScene = true;

			if (GameUI.Instance != null && transition != GameTransitionType.HardCut)
				yield return GameUI.Instance.FadeOut();

			GameGlobals.Instance.VisitScene(sceneId);
			AsyncOperation load = SceneManager.LoadSceneAsync(sceneId);
			while (load != null && !load.isDone)
				yield return null;

			GameSaveSystem.Save(GameGlobals.Instance);

			if (GameUI.Instance != null && transition != GameTransitionType.HardCut)
				yield return GameUI.Instance.FadeIn();

			IsChangingScene = false;
		}
	}
}
