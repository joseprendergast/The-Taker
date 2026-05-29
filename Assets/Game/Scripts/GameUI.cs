using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameFoundation
{
	[DefaultExecutionOrder(-700)]
	public class GameUI : MonoBehaviour
	{
		public static GameUI Instance { get; private set; }

		[Header("Title/Menu")]
		[SerializeField] GameObject titleMenu;
		[SerializeField] GameObject pauseMenu;
		[SerializeField] GameObject settingsPanel;

		[Header("Compact UI")]
		[SerializeField] Text hotspotLabel;
		[SerializeField] Text subtitleText;
		[SerializeField] Text debugText;
		[SerializeField] GameObject inventoryStrip;
		[SerializeField] CanvasGroup fadeOverlay;

		[SerializeField] float fadeSeconds = 0.35f;

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
			HideHotspotLabel();
			HideSubtitle();
		}

		public void ShowHotspotLabel(string label)
		{
			if (hotspotLabel == null)
				return;

			hotspotLabel.text = label;
			hotspotLabel.gameObject.SetActive(true);
		}

		public void HideHotspotLabel()
		{
			if (hotspotLabel != null)
				hotspotLabel.gameObject.SetActive(false);
		}

		public void ShowSubtitle(string speaker, string text)
		{
			if (subtitleText == null)
				return;

			subtitleText.text = string.IsNullOrWhiteSpace(speaker) ? text : $"{speaker}: {text}";
			subtitleText.gameObject.SetActive(true);
		}

		public void HideSubtitle()
		{
			if (subtitleText != null)
				subtitleText.gameObject.SetActive(false);
		}

		public void RefreshInventory()
		{
			if (inventoryStrip != null)
				inventoryStrip.SetActive(true);
		}

		public void SetInventoryVisible(bool visible)
		{
			if (inventoryStrip != null)
				inventoryStrip.SetActive(visible);
		}

		public IEnumerator FadeOut()
		{
			yield return FadeTo(1f);
		}

		public IEnumerator FadeIn()
		{
			yield return FadeTo(0f);
		}

		IEnumerator FadeTo(float target)
		{
			if (fadeOverlay == null)
				yield break;

			fadeOverlay.blocksRaycasts = true;
			float start = fadeOverlay.alpha;
			float timer = 0f;
			while (timer < fadeSeconds)
			{
				timer += Time.unscaledDeltaTime;
				fadeOverlay.alpha = Mathf.Lerp(start, target, timer / fadeSeconds);
				yield return null;
			}

			fadeOverlay.alpha = target;
			fadeOverlay.blocksRaycasts = target > 0.01f;
		}

		public void SetDebugText(string text)
		{
			if (debugText != null)
				debugText.text = text;
		}
	}
}
