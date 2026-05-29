using System.Collections;
using UnityEngine;

namespace GameFoundation
{
	public class GameAtmosphere : MonoBehaviour
	{
		[SerializeField] CanvasGroup rainOverlay;
		[SerializeField] CanvasGroup fogOverlay;
		[SerializeField] CanvasGroup hazeOverlay;
		[SerializeField] Light flickerLight;
		[SerializeField] AudioSource ambienceSource;
		[SerializeField] AudioSource sfxSource;
		[SerializeField] float flickerAmount = 0.08f;

		Vector3 cameraStart;

		void Start()
		{
			if (Camera.main != null)
				cameraStart = Camera.main.transform.localPosition;
		}

		void Update()
		{
			if (flickerLight != null)
				flickerLight.intensity += Random.Range(-flickerAmount, flickerAmount) * Time.deltaTime;
		}

		public void SetRain(float alpha) => SetOverlay(rainOverlay, alpha);
		public void SetFog(float alpha) => SetOverlay(fogOverlay, alpha);
		public void SetHaze(float alpha) => SetOverlay(hazeOverlay, alpha);
		public void PlayRoomTone(AudioClip clip) => PlayLoop(ambienceSource, clip);
		public void PlayTransitionSting(AudioClip clip) => PlayOneShot(clip);
		public void PlayUiClick(AudioClip clip) => PlayOneShot(clip);
		public void Shake(float seconds = 0.18f, float strength = 0.08f) => StartCoroutine(ShakeRoutine(seconds, strength));

		void SetOverlay(CanvasGroup group, float alpha)
		{
			if (group != null)
				group.alpha = Mathf.Clamp01(alpha);
		}

		void PlayLoop(AudioSource source, AudioClip clip)
		{
			if (source == null || clip == null)
				return;

			source.clip = clip;
			source.loop = true;
			source.Play();
		}

		void PlayOneShot(AudioClip clip)
		{
			if (sfxSource != null && clip != null)
				sfxSource.PlayOneShot(clip);
		}

		IEnumerator ShakeRoutine(float seconds, float strength)
		{
			if (Camera.main == null)
				yield break;

			Transform cam = Camera.main.transform;
			float timer = 0f;
			while (timer < seconds)
			{
				timer += Time.deltaTime;
				cam.localPosition = cameraStart + (Vector3)Random.insideUnitCircle * strength;
				yield return null;
			}
			cam.localPosition = cameraStart;
		}
	}
}
