using System.Collections;
using UnityEngine;

namespace GameFoundation
{
	[DefaultExecutionOrder(-800)]
	public class GameDialogue : MonoBehaviour
	{
		public static GameDialogue Instance { get; private set; }

		Coroutine activeLine;
		bool skipRequested;

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

		void Update()
		{
			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
				skipRequested = true;
		}

		public Coroutine Say(string speaker, string text, string flag = null, bool playOnce = false, float holdSeconds = 2.5f)
		{
			return Say(new SubtitleLineData { speaker = speaker, text = text, flagOnFirstPlay = flag, playOnce = playOnce, holdSeconds = holdSeconds });
		}

		public Coroutine Say(SubtitleLineData line)
		{
			if (line == null || string.IsNullOrWhiteSpace(line.text))
				return null;

			if (line.playOnce && !string.IsNullOrWhiteSpace(line.flagOnFirstPlay) && GameGlobals.Instance.HasFlag(line.flagOnFirstPlay))
				return null;

			if (activeLine != null)
				StopCoroutine(activeLine);

			activeLine = StartCoroutine(SayRoutine(line));
			return activeLine;
		}

		IEnumerator SayRoutine(SubtitleLineData line)
		{
			skipRequested = false;
			GameUI.Instance?.ShowSubtitle(line.speaker, line.text);

			if (!string.IsNullOrWhiteSpace(line.flagOnFirstPlay))
				GameGlobals.Instance.SetDialogueFlag(line.flagOnFirstPlay);

			float timer = 0f;
			float hold = Mathf.Max(0.2f, line.holdSeconds);
			while (timer < hold && !skipRequested)
			{
				timer += Time.unscaledDeltaTime;
				yield return null;
			}

			GameUI.Instance?.HideSubtitle();
			activeLine = null;
		}
	}
}
