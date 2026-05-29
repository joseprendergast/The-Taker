using UnityEngine;

namespace GameFoundation
{
	public class GameRoomInput : MonoBehaviour
	{
		[SerializeField] Camera inputCamera;
		[SerializeField] LayerMask hotspotMask = ~0;

		void Awake()
		{
			if (inputCamera == null)
				inputCamera = Camera.main;
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				GameInventory.Instance?.ClearSelection();

			if (Input.GetMouseButtonDown(0))
				RouteClick(ContextualActionType.Context);
			else if (Input.GetMouseButtonDown(1))
				RouteClick(ContextualActionType.Inspect);
		}

		void RouteClick(ContextualActionType action)
		{
			GameHotspot hotspot = HotspotUnderPointer();
			if (hotspot == null)
			{
				if (action == ContextualActionType.Context)
					GameInventory.Instance?.ClearSelection();
				return;
			}

			if (action == ContextualActionType.Inspect)
				hotspot.Inspect();
			else if (GameInventory.Instance != null && !string.IsNullOrWhiteSpace(GameInventory.Instance.SelectedItemId))
				hotspot.UseSelectedItem();
			else
				hotspot.HandleContextAction();
		}

		GameHotspot HotspotUnderPointer()
		{
			Camera cam = inputCamera != null ? inputCamera : Camera.main;
			if (cam == null)
				return null;

			Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
			Collider2D hit = Physics2D.OverlapPoint(world, hotspotMask);
			return hit != null ? hit.GetComponentInParent<GameHotspot>() : null;
		}
	}
}
