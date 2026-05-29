using UnityEngine;
using System.Collections;
using PowerScript;
using PowerTools.Quest;

public class RoomForest : RoomScript<RoomForest>
{
	public void OnEnterRoom()
	{
		G.InventoryBar.Show();
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		C.Dave.WalkToBG(Point("EntryWalk"));

		if (FirstTimeVisited && EnteredFromEditor == false)
		{
			yield return C.Display("The graveyard gives Fanto no comfort zone. Familiar stones shift when the fog hides them.");
			yield return C.Display("Left click to walk or act. Right click to inspect. Use the well when you are ready to judge.");
			yield return C.Barney.Say("Little Judge, mercy and punishment both leave marks.");
		}

		yield return DescribeCurrentCase();
	}

	IEnumerator DescribeCurrentCase()
	{
		if (Globals.AllGhostsJudged)
		{
			yield return BeginFinalJudgement();
			yield break;
		}

		GlobalScript.GhostCase ghost = Globals.CurrentCase;
		yield return C.Display("Current soul: " + ghost.Name + "\n" + ghost.Tone + " in " + ghost.Scenario + ".");
		yield return C.Display(ghost.Encounter);
	}

	public IEnumerator OnInteractHotspotForest(Hotspot hotspot)
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();

		if (Globals.AllGhostsJudged)
		{
			yield return C.Display("The trees are quiet. Even the false faces in the fog seem to be waiting for your sentence.");
			yield break;
		}

		GlobalScript.GhostCase ghost = Globals.CurrentCase;
		yield return C.Display("You search " + ghost.Scenario + ".");
		yield return C.Display("Evidence: " + ghost.Crime + ".");
		Globals.RecordEvidence();
		yield return C.Display("Evidence gathered: " + Globals.m_evidenceFound + "/3.");
	}

	public IEnumerator OnInteractHotspotCave(Hotspot hotspot)
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();

		if (Globals.AllGhostsJudged)
		{
			yield return C.Barney.Say("The last pending judgement is not among the dead.");
			yield break;
		}

		GlobalScript.GhostCase ghost = Globals.CurrentCase;
		yield return C.Display("A colder path opens beside the graves.");
		yield return C.Display("Moral doubt: " + ghost.Doubt + ".");

		if (Globals.m_dollEncounters < Globals.m_currentGhost + 1)
		{
			Globals.RecordDollEncounter();
			yield return C.Barney.Say("Doll passed this way. She left a charm and a question: why must every law be black or white?");
		}
	}

	public IEnumerator OnInteractPropBucket(Prop prop)
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();

		if (Globals.AllGhostsJudged)
		{
			yield return C.Display("The last scraps of evidence are ash-cold in Fanto's hand.");
			yield break;
		}

		Globals.RecordEvidence();
		yield return C.Display("Fanto pockets a memory shard.");
		yield return C.Display("Evidence gathered: " + Globals.m_evidenceFound + "/3.");
	}

	public IEnumerator OnInteractPropWell(Prop prop)
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();

		if (Globals.AllGhostsJudged)
		{
			yield return BeginFinalJudgement();
			yield break;
		}

		GlobalScript.GhostCase ghost = Globals.CurrentCase;
		yield return C.Display("The well becomes a black mirror. " + ghost.Name + " is forced into the reflection.");

		if (Globals.m_evidenceFound < 2)
			yield return C.Display("You have little evidence. You may still judge, but the sentence will feel less certain.");

		yield return C.Display("Battle: avoid the ghost's memory echoes, then strike with the scythe when the room breathes in.");
		yield return GuiPrompt.Script.WaitForPrompt("Judge " + ghost.Name + ":", "HEAVEN", "HELL");

		GlobalScript.eJudgement judgement = GuiPrompt.Script.Result ? GlobalScript.eJudgement.Heaven : GlobalScript.eJudgement.Hell;
		Globals.JudgeCurrentGhost(judgement);

		yield return C.Display(ghost.Name + " is sent to " + (judgement == GlobalScript.eJudgement.Heaven ? "Heaven." : "Hell."));

		if (judgement == GlobalScript.eJudgement.Hell && Globals.HellCount == 1)
			yield return C.Barney.Say("Do you understand the infinite magnitude of the word forever?");

		if (Globals.AllGhostsJudged)
			yield return BeginFinalJudgement();
		else
			yield return DescribeCurrentCase();
	}

	public IEnumerator OnInteractHotspotSky(Hotspot hotspot)
	{
		yield return C.FaceClicked();

		if (Globals.AllGhostsJudged == false)
		{
			yield return C.Display("Judged souls: " + Globals.m_currentGhost + "/" + Globals.TotalGhosts + "\nSent to Hell: " + Globals.HellCount + "\nDoll encounters: " + Globals.m_dollEncounters + "/" + Globals.TotalGhosts);
			yield break;
		}

		yield return C.Display("Ending unlocked: " + Globals.BuildEndingName());
	}

	public IEnumerator OnLookAtHotspotForest(IHotspot hotspot)
	{
		yield return C.FaceClicked();
		yield return C.Display("Dead trees keep harmless faces in the fog. Up close, they are only wood. Usually.");
	}

	public IEnumerator OnLookAtPropWell(IProp prop)
	{
		yield return C.FaceClicked();
		yield return C.Display("The well is a judgement mirror. It shows crimes, doubts, and the judge behind them.");
	}

	IEnumerator OnInteractCharacterBarney(ICharacter character)
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();

		if (Globals.AllGhostsJudged)
		{
			yield return C.Barney.Say("Nine ghosts have been judged. Now tell me, little Judge: how many souls have you sent to Hell?");
			yield return C.Display("Answer: " + Globals.HellCount + ".");
		}
		else
		{
			yield return C.Barney.Say("Find the ghost. Fight the ghost. Judge the ghost. The order is simple. The guilt is not.");
		}
	}

	IEnumerator BeginFinalJudgement()
	{
		if (Globals.m_finalChoiceMade)
		{
			yield return C.Display("Ending unlocked: " + Globals.BuildEndingName());
			yield break;
		}

		yield return C.Barney.Say("You have accomplished your duty. Nine ghosts have been judged.");
		yield return C.Barney.Say("However... there is yet a pending judgement.");
		yield return C.Display("The Archangel turns. His scythe points at Fanto.");
		yield return GuiPrompt.Script.WaitForPrompt("Judge yourself:", "SALVATION", "DAMNATION");

		Globals.m_selfJudgement = GuiPrompt.Script.Result ? GlobalScript.eJudgement.Heaven : GlobalScript.eJudgement.Hell;
		Globals.m_finalChoiceMade = true;
		Globals.m_progress = GlobalScript.eProgress.Complete;

		if (Globals.m_selfJudgement == GlobalScript.eJudgement.Heaven)
		{
			yield return C.Barney.Say("Punishing others is easy. Punishing oneself is harder.");
			yield return C.Display("Final battle: the Archangel reveals the Supreme Arbiter beneath the mask.");
		}
		else
		{
			yield return C.Display("Fanto accepts the weight of every sentence he delivered.");
		}

		yield return C.Display("Ending unlocked: " + Globals.BuildEndingName());
	}
}
