using System.Collections.Generic;
using UnityEngine;

namespace GameFoundation
{
	[RequireComponent(typeof(Collider2D))]
	public class GameHotspot : MonoBehaviour
	{
		public string hotspotId;
		public string displayName;
		[TextArea(2, 4)] public string inspectLine;
		[TextArea(2, 4)] public string repeatedInspectLine;
		[TextArea(2, 4)] public string interactLine;
		public List<string> requiredFlags = new List<string>();
		public List<string> setFlags = new List<string>();
		public string collectItemId;
		public string requiredItemId;
		public string roomChangeSceneId;
		public GameTransitionType transitionType = GameTransitionType.FadeToBlack;
		public SubtitleLineData dialogueBeat;

		bool inspected;
		bool hovering;

		void OnMouseEnter()
		{
			hovering = true;
			GameUI.Instance?.ShowHotspotLabel(string.IsNullOrWhiteSpace(displayName) ? hotspotId : displayName);
		}

		void OnMouseExit()
		{
			hovering = false;
			GameUI.Instance?.HideHotspotLabel();
		}

		void Update()
		{
			if (hovering && Input.GetMouseButtonDown(1))
				Inspect();
		}

		void OnMouseDown()
		{
			HandleContextAction();
		}

		public void HandleContextAction()
		{
			if (GameInventory.Instance != null && !string.IsNullOrWhiteSpace(GameInventory.Instance.SelectedItemId))
				UseSelectedItem();
			else
				Interact();
		}

		public void Inspect()
		{
			string line = inspected && !string.IsNullOrWhiteSpace(repeatedInspectLine) ? repeatedInspectLine : inspectLine;
			inspected = true;
			GameDialogue.Instance?.Say(null, line, $"{hotspotId}.inspected", false, 2.2f);
		}

		public void Interact()
		{
			if (!RequirementsMet())
			{
				GameDialogue.Instance?.Say(null, "Nothing changes yet.", null, false, 1.6f);
				return;
			}

			ApplyEffects();

			if (dialogueBeat != null && !string.IsNullOrWhiteSpace(dialogueBeat.text))
				GameDialogue.Instance?.Say(dialogueBeat);
			else if (!string.IsNullOrWhiteSpace(interactLine))
				GameDialogue.Instance?.Say(null, interactLine, null, false, 2f);

			if (!string.IsNullOrWhiteSpace(roomChangeSceneId))
				GameSceneManager.Instance?.ChangeScene(roomChangeSceneId, transitionType);
		}

		public void UseSelectedItem()
		{
			string selected = GameInventory.Instance.SelectedItemId;
			if (!string.IsNullOrWhiteSpace(requiredItemId) && selected != requiredItemId)
			{
				GameDialogue.Instance?.Say(null, "That does not fit here.", null, false, 1.5f);
				return;
			}

			Interact();
			GameInventory.Instance.ClearSelection();
		}

		public void HandleFromPowerQuest(ContextualActionType action)
		{
			switch (action)
			{
				case ContextualActionType.Inspect:
					Inspect();
					break;
				case ContextualActionType.UseItem:
					UseSelectedItem();
					break;
				default:
					Interact();
					break;
			}
		}

		bool RequirementsMet()
		{
			foreach (string flag in requiredFlags)
				if (!GameGlobals.Instance.HasFlag(flag))
					return false;

			if (!string.IsNullOrWhiteSpace(requiredItemId) && !GameGlobals.Instance.HasItem(requiredItemId))
				return false;

			return true;
		}

		void ApplyEffects()
		{
			foreach (string flag in setFlags)
				GameGlobals.Instance.SetFlag(flag);

			if (!string.IsNullOrWhiteSpace(collectItemId))
				GameInventory.Instance?.Collect(collectItemId);
		}
	}
}
