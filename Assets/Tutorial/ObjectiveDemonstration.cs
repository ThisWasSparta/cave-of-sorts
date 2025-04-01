using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialScenarios/ObjectiveDemonstration")]
public class ObjectiveDemonstration : TutorialScenario {
	private const float X_OFFSET = -65f;

	public override IEnumerator ProcessScenario(TutorialManager manager) {
		this.tutorialManager = manager;
		placeFlag = false;
		matchFlag = false;
		tutorialManager.ChangeScreenCoverState(false);
		WaitForSeconds pacingDelay = new WaitForSeconds(0.2f);
		var gridManager = GridManager.Instance;
		var inputSystem = GridManager.Instance.InputSystem;
		inputSystem.SetControlState(false);
		inputSystem.SetupTutorial(allowedPlacementCoords);
		gridManager.InstantiateGrid(CreateInstance<LevelContentData>(), gridData);
		gridManager.DealerBehaviour.SetHitButtonState(false);
		var chips = new Tuple<ChipType, int>[2] {
			new Tuple<ChipType, int>(ChipType.Green, 4),
			new Tuple<ChipType, int>(ChipType.Red, 4)
		};
		gridManager.CreateSpecificStack(new Vector3Int(-1, 0, 0), chips);
		gridManager.CreateSpecificStack(new Vector3Int(1, 0, 0), chips);
		var objectiveTracker = tutorialManager.RetrieveObjectiveTracker();
		objectiveTracker.SetTutorialFlag();

		var objectives = new List<TrackedObjective> {
			new TrackedObjective(ObjectiveType.ColouredChipQuota, ChipType.Red, 8),
			new TrackedObjective(ObjectiveType.ColouredChipQuota, ChipType.Green, 8)
		};
		var starData = new bool[3] {
			false,
			false,
			false
		};
		objectiveTracker.Setup(objectives, null, starData);

		yield return tutorialManager.DrawOrCloseCurtain(true, false);

		tutorialManager.PointGloveAnimation(0);

		dialogue = new string[2] {
			$"Have a gander up there in the corner! There you can see what a level requires you to do to win. In this case, clear {objectives[0].requiredAmount} {objectives[0].chipType} ones and {objectives[1].requiredAmount} ones.",
			"Let me show you what that looks like. I'll go ahead and give you another stack to put on the board."
		};
		yield return tutorialManager.StartDialogue(dialogue);
		tutorialManager.StopGloveAnimation();

		Vector3 stackPos;
		gridManager.DealerBehaviour.DealSpecificChipStack(chips, out stackPos);
		inputSystem.SetControlState(true);

		placeFlag = false;
		while (!placeFlag) {
			yield return null;
		}
		tutorialManager.StopGloveAnimation();

		inputSystem.RegisterDelayedStackPlacement();

		matchFlag = false;
		while (!matchFlag) {
			yield return null;
		}

		inputSystem.SetControlState(false);

		dialogue = new string[3] {
			"See? Simple as. Clearing these objectives will finish the level and earn you the first out of three stars.",
			"Pay close attention to the stacks you're dealt. Set it all up right, and you might just clear the whole board with one move!",
			"Next I'll show you what to do to earn the other two. Nearly there now!"
		};
		yield return tutorialManager.StartDialogue(dialogue);
	}
}