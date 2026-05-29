using UnityEngine;
using System.Collections;
using PowerScript;
using PowerTools.Quest;

public class RoomTitle : RoomScript<RoomTitle>
{
	bool m_menuOpen = false;
	bool m_continueAvailable = false;

	public void OnEnterRoom()
	{
		G.InventoryBar.Hide();
		m_menuOpen = false;
		m_continueAvailable = E.GetSaveSlotData().Count > 0;

		SetPropLabel(Prop("New"), "PRESS START");
		Prop("New").Description = "Press start";
		Prop("New").SetPosition(0, -51.1f);
		Prop("New").Enable();

		SetPropLabel(Prop("Continue"), "CONTINUE");
		Prop("Continue").Disable();
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		E.StartCutscene();

		Prop("Title").Visible = true;
		yield return Prop("Title").Fade(0, 1, 0.5f);
		yield return Prop("New").Fade(0, 1, 0.35f);

		E.EndCutscene();
	}

	public IEnumerator OnInteractPropNew(Prop prop)
	{
		if (m_menuOpen == false)
		{
			OpenMainMenu();
			yield return E.ConsumeEvent;
			yield break;
		}

		Globals.ResetRun();
		G.InventoryBar.Show();
		E.ChangeRoomBG(R.Forest);
		yield return E.ConsumeEvent;
	}

	public IEnumerator OnInteractPropContinue(Prop prop)
	{
		E.RestoreLastSave();
		yield return E.ConsumeEvent;
	}

	public IEnumerator OnLookAtPropNew(IProp prop)
	{
		yield return C.Display(m_menuOpen ? "Begin a new judgement." : "The dead are waiting.");
	}

	public IEnumerator OnLookAtPropContinue(IProp prop)
	{
		yield return C.Display("Return to the last saved judgement.");
	}

	void OpenMainMenu()
	{
		m_menuOpen = true;

		SetPropLabel(Prop("New"), "NEW GAME");
		Prop("New").Description = "Start a new game";
		Prop("New").SetPosition(0, m_continueAvailable ? -47.1f : -51.1f);
		Prop("New").Enable();

		if (m_continueAvailable)
		{
			SetPropLabel(Prop("Continue"), "CONTINUE");
			Prop("Continue").Description = "Continue a previous game";
			Prop("Continue").Enable();
		}
	}

	void SetPropLabel(IProp prop, string text)
	{
		QuestText questText = prop.Instance.GetComponentInChildren<QuestText>(true);
		if (questText != null)
			questText.text = text;

		TextMesh textMesh = prop.Instance.GetComponentInChildren<TextMesh>(true);
		if (textMesh != null)
			textMesh.text = text;
	}
}
