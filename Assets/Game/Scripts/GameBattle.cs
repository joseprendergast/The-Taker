using System;
using UnityEngine;

namespace GameFoundation
{
	public class GameBattle : MonoBehaviour
	{
		[SerializeField] GameObject battleOverlay;

		public event Action BattleStarted;
		public event Action<bool> BattleEnded;

		public void StartBattle()
		{
			if (battleOverlay != null)
				battleOverlay.SetActive(true);
			BattleStarted?.Invoke();
		}

		public void EndBattle(bool victory)
		{
			if (battleOverlay != null)
				battleOverlay.SetActive(false);
			BattleEnded?.Invoke(victory);
		}
	}
}
