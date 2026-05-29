using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerScript;
using PowerTools.Quest;

public partial class GlobalScript : GlobalScriptBase<GlobalScript>
{
	public enum eJudgement
	{
		Unjudged,
		Heaven,
		Hell
	}

	public enum eProgress
	{
		Intro,
		Hunting,
		FinalChoice,
		Complete
	}

	[System.Serializable]
	public class GhostCase
	{
		public string Name;
		public string Tone;
		public string Scenario;
		public string Crime;
		public string Doubt;
		public string Encounter;
	}

	public eProgress m_progress = eProgress.Intro;
	public int m_currentGhost = 0;
	public int m_evidenceFound = 0;
	public int m_dollEncounters = 0;
	public bool m_archangelWarningSeen = false;
	public bool m_finalChoiceMade = false;
	public eJudgement m_selfJudgement = eJudgement.Unjudged;
	public eJudgement[] m_judgements = new eJudgement[9];

	GhostCase[] m_cases =
	{
		new GhostCase { Name = "The Innocent", Tone = "funny tutorial", Scenario = "the graveyard", Crime = "he abandoned his dog in the snow", Doubt = "one cruel act against an otherwise ordinary life", Encounter = "A shy shape slips between the graves and tries to flee." },
		new GhostCase { Name = "The Everlasting Valentine", Tone = "spooky battle", Scenario = "the graveyard", Crime = "she murdered four women near her husband", Doubt = "love turned possessive enough to kill", Encounter = "A bride in black hair sobs until the sob becomes a blade." },
		new GhostCase { Name = "The Poltergeist", Tone = "spooky puzzle", Scenario = "the abandoned house", Crime = "her games killed seventeen people", Doubt = "she is a lonely child who never understood death", Encounter = "Candles gutter, mirrors cloud, and a laughing little girl appears." },
		new GhostCase { Name = "The Doppelganger", Tone = "comic riddle", Scenario = "the mirrored graveyard", Crime = "mischief without malice", Doubt = "a harmless ghost can still be condemned by a careless judge", Encounter = "Another Fanto copies your stance a half-second too late." },
		new GhostCase { Name = "The Faceless Lady", Tone = "pure horror", Scenario = "the forest", Crime = "she killed thirty-nine innocents while seeking revenge", Doubt = "grief and madness hollowed her out", Encounter = "A white dress hangs between the trees where no body should fit." },
		new GhostCase { Name = "The Forgotten", Tone = "mystery adventure", Scenario = "the crypt", Crime = "ancient crimes no living soul remembers", Doubt = "can a forgotten life still deserve eternal punishment?", Encounter = "A weak shadow waits beside a tomb older than prayer." },
		new GhostCase { Name = "The Guilty", Tone = "pure battle", Scenario = "the torture engine", Crime = "he ordered millions into death", Doubt = "his suffering explains him, but does not excuse him", Encounter = "Chains pull a colossal wraith upright and the lesser souls scream." },
		new GhostCase { Name = "The Evil", Tone = "puzzle battle", Scenario = "the forest", Crime = "he sacrificed children for demonic power", Doubt = "evil can be smaller in number and still feel absolute", Encounter = "The sleeping face in the twisted tree opens one amber eye." },
		new GhostCase { Name = "Seelvia", Tone = "dramatic finale", Scenario = "the graveyard", Crime = "the truth is personal, partial, and painful", Doubt = "Fanto wants her saved before he wants her judged", Encounter = "A blonde ghost smiles like memory, then raises her guard." },
	};

	public GhostCase CurrentCase
	{
		get { return m_cases[Mathf.Clamp(m_currentGhost, 0, m_cases.Length - 1)]; }
	}

	public int TotalGhosts
	{
		get { return m_cases.Length; }
	}

	public int HellCount
	{
		get
		{
			int count = 0;
			foreach (eJudgement judgement in m_judgements)
				if (judgement == eJudgement.Hell)
					count++;
			return count;
		}
	}

	public bool AllGhostsJudged
	{
		get { return m_currentGhost >= m_cases.Length; }
	}

	public void OnGameStart()
	{
		ResetRun();
	}

	public void ResetRun()
	{
		m_progress = eProgress.Intro;
		m_currentGhost = 0;
		m_evidenceFound = 0;
		m_dollEncounters = 0;
		m_archangelWarningSeen = false;
		m_finalChoiceMade = false;
		m_selfJudgement = eJudgement.Unjudged;
		for (int i = 0; i < m_judgements.Length; ++i)
			m_judgements[i] = eJudgement.Unjudged;
	}

	public void OnPostRestore(int version)
	{
	}

	public void OnEnterRoom()
	{
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		yield return E.Break;
	}

	public IEnumerator OnExitRoom(IRoom oldRoom, IRoom newRoom)
	{
		yield return E.Break;
	}

	public IEnumerator UpdateBlocking()
	{
		yield return E.Break;
	}

	public void Update()
	{
	}

	public void UpdateNoPause()
	{
		UpdateInput();
	}

	void UpdateInput()
	{
		if (E.Paused == false)
		{
			if (Input.GetKeyUp(KeyCode.Escape))
				E.SkipCutscene();
			if (Input.GetMouseButtonDown(0))
				E.SkipDialog(true);
			if (Input.GetKey(KeyCode.Escape) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space))
				E.SkipDialog(false);
		}

		if (E.GetBlocked() == false && E.Paused == false && Input.GetKeyDown(KeyCode.F1))
			G.Options.Show();
	}

	public IEnumerator OnAnyClick()
	{
		yield return E.Break;
	}

	public IEnumerator OnWalkTo()
	{
		yield return E.Break;
	}

	public void OnMouseClick(bool leftClick, bool rightClick)
	{
		bool mouseOverSomething = E.GetMouseOverClickable() != null;

		if (E.GetMouseOverType() == eQuestClickableType.Gui || Cursor.NoneCursorActive)
			return;

		if (leftClick)
			E.ProcessClick(mouseOverSomething ? eQuestVerb.Use : eQuestVerb.Walk);
		else if (rightClick && mouseOverSomething)
			E.ProcessClick(eQuestVerb.Look);
	}

	public void RecordEvidence()
	{
		m_evidenceFound = Mathf.Clamp(m_evidenceFound + 1, 0, 3);
	}

	public void RecordDollEncounter()
	{
		m_dollEncounters = Mathf.Clamp(m_dollEncounters + 1, 0, TotalGhosts);
	}

	public void JudgeCurrentGhost(eJudgement judgement)
	{
		if (AllGhostsJudged)
			return;

		m_judgements[m_currentGhost] = judgement;
		m_currentGhost++;
		m_evidenceFound = 0;
		m_progress = AllGhostsJudged ? eProgress.FinalChoice : eProgress.Hunting;
	}

	public string BuildEndingName()
	{
		bool seelviaHell = m_judgements[8] == eJudgement.Hell;

		if (m_dollEncounters >= TotalGhosts)
			return "DOLL'S ENDING";
		if (HellCount == 0)
			return "THE GUARDIAN ANGEL";
		if (seelviaHell && m_selfJudgement == eJudgement.Hell)
			return "LOVE IN HELL";
		if (seelviaHell && m_selfJudgement == eJudgement.Heaven)
			return "THE GUILTY ENDING";
		if (seelviaHell == false && m_selfJudgement == eJudgement.Hell)
			return "THE RIGHT THING";
		return "HAPPILY EVER AFTER";
	}
}
