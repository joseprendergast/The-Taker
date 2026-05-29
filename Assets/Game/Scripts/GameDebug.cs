using UnityEngine;

namespace GameFoundation
{
	public class GameDebug : MonoBehaviour
	{
		[SerializeField] bool debugEnabled = true;
		[SerializeField] GameObject debugPanel;
		[SerializeField] string[] starterRooms = { "RoomTitle", "RoomPrototypeA", "RoomPrototypeB", "RoomPrototypeC" };
		[SerializeField] string grantItemId = "debug-item";
		[SerializeField] bool revealHotspots;

		void Update()
		{
			if (!debugEnabled)
				return;

			if (Input.GetKeyDown(KeyCode.F1))
				TogglePanel();

			for (int i = 0; i < starterRooms.Length && i < 9; ++i)
				if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
					GameSceneManager.Instance?.ChangeScene(starterRooms[i], GameTransitionType.HardCut);

			if (Input.GetKeyDown(KeyCode.G))
				GameInventory.Instance?.Collect(grantItemId);

			if (Input.GetKeyDown(KeyCode.R))
				ResetSaveAndProgress();

			if (Input.GetKeyDown(KeyCode.H))
				revealHotspots = !revealHotspots;

			if (Input.GetKeyDown(KeyCode.C))
				GameInventory.Instance?.ClearInventoryForDebug();

			GameUI.Instance?.SetDebugText($"Scene: {GameGlobals.Instance?.CurrentScene}\nHotspots: {(revealHotspots ? "revealed" : "normal")}");
		}

		public void TogglePanel()
		{
			if (debugPanel != null)
				debugPanel.SetActive(!debugPanel.activeSelf);
		}

		public void ResetSaveAndProgress()
		{
			GameSaveSystem.Reset();
			GameGlobals.Instance?.ResetProgress();
		}
	}
}
