using UnityEngine;

namespace GameFoundation
{
	public class GameRoom : MonoBehaviour
	{
		public GameSceneDefinition definition = new GameSceneDefinition();

		void Start()
		{
			if (!string.IsNullOrWhiteSpace(definition.sceneId))
				GameGlobals.Instance?.VisitScene(definition.sceneId);

			if (definition.narrativeBeats != null && definition.narrativeBeats.Count > 0)
				GameDialogue.Instance?.Say(definition.narrativeBeats[0]);
		}

		public bool RequirementsMet()
		{
			if (definition.requiredFlags == null)
				return true;

			foreach (string flag in definition.requiredFlags)
				if (!GameGlobals.Instance.HasFlag(flag))
					return false;

			return true;
		}

		public void MarkComplete()
		{
			if (!string.IsNullOrWhiteSpace(definition.completionFlag))
				GameGlobals.Instance.SetFlag(definition.completionFlag);
		}
	}
}
