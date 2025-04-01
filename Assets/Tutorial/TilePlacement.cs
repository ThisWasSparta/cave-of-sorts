using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialScenarios/TilePlacement")]
public class TilePlacement : TutorialScenario {
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
		gridManager.CreateSpecificStack(new Vector3Int(0, 1, 0), ChipType.Red, 2);
		gridManager.CreateSpecificStack(new Vector3Int(-1, -1, 0), ChipType.Red, 2);
		gridManager.CreateSpecificStack(new Vector3Int(-1, 1, 0), ChipType.Green, 3);

		tutorialManager.ChangeScreenCoverState(false);
		yield return tutorialManager.DrawOrCloseCurtain(true, false);
		yield return new WaitForSeconds(0.1f);

		dialogue = new string[2] { // yes yes, this isn't flexible/scalable, but i'm not translating the game anyway, whatevs
			"Let me give you the rundown, chum.",
			"These gems are the treasure that needs sorting. I'll hand you a stack of them. Go ahead, drag and drop it in place. Tapping works too!"
		};
		yield return tutorialManager.StartDialogue(dialogue);

		Vector3 stackPos;
		gridManager.DealerBehaviour.DealSpecificChipStack(ChipType.Red, 2, out stackPos);
		inputSystem.SetControlState(true);
		tutorialManager.DragGloveAnimation(stackPos, gridManager.GridToWorldPos(allowedPlacementCoords[0]));

		while (!placeFlag) {
			yield return null;
		}

		tutorialManager.StopGloveAnimation();
		inputSystem.SetControlState(false);

		dialogue = new string[2] {
			"Just like that! Well done, chum.",
			"Now, watch what happens..."
		};
		yield return tutorialManager.StartDialogue(dialogue);
		yield return pacingDelay;

		inputSystem.RegisterDelayedStackPlacement();

		while (!matchFlag) {
			yield return null;
		}

		dialogue = new string[2] {
			"See? Sorted! It's as easy as can be!",
			"Now watch what happens when you stack 10 of them on top of each other!"
		};
		yield return tutorialManager.StartDialogue(dialogue);

		gridManager.DealerBehaviour.DealSpecificChipStack(ChipType.Red, 4, out stackPos);
		tutorialManager.DragGloveAnimation(stackPos, gridManager.GridToWorldPos(allowedPlacementCoords[0]));
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

		dialogue = new string[2] {
			"Look at that. Isn't the board just so much neater now?",
			"Next I'll tell you how you go about finishing a level. Then you'll be all set!"
		};

		yield return tutorialManager.StartDialogue(dialogue);
		yield return tutorialManager.DrawOrCloseCurtain(true, true);
	}
}