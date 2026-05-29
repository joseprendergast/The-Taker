using UnityEngine;
using System.Collections;
using PowerScript;
using PowerTools.Quest;

public class RoomTitle : RoomScript<RoomTitle>
{
	public void OnEnterRoom()
	{
		G.InventoryBar.Hide();
		Prop("Continue").Disable();
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		E.StartCutscene();

		Prop("Title").Visible = true;
		yield return Prop("Title").Fade(0, 1, 0.5f);
		yield return C.Display("THE TAKER\nA desktop PowerQuest prototype");
		yield return C.Display("Find nine lost souls. Learn enough to judge them. Decide whether Heaven or Hell is justice.");

		Prop("New").Enable();
		yield return C.Display("Click NEW to begin.");

		E.EndCutscene();
	}

	public IEnumerator OnInteractPropNew(Prop prop)
	{
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
}
