using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialScenarios/StarDemonstration")]
public class StarDemonstration : TutorialScenario {
	public override IEnumerator ProcessScenario(TutorialManager manager) {
		this.tutorialManager = manager;
		placeFlag = false;
		matchFlag = false;
		//tutorialManager.ChangeScreenCoverState(false);



		var gridManager = GridManager.Instance;
		var inputSystem = GridManager.Instance.InputSystem;

		gridManager.ClearGrid();
		gridManager.InstantiateGrid(CreateInstance<LevelContentData>(), gridData);
		inputSystem.SetControlState(false);
		inputSystem.SetupTutorial(allowedPlacementCoords);
		gridManager.InstantiateGrid(CreateInstance<LevelContentData>(), gridData);
		gridManager.DealerBehaviour.SetHitButtonState(false);

		// set up grid, make it a bit bigger this time? gives a better representation of the levels will look like
		// 

		// animationCoordinator.PlayCurseAnimation()

		while (!placeFlag) {
			yield return null;
		}

		// introduce cursed tiles

		// wait for a half second or more
		// 'chugga chugga' as the chains appear

		// have smog chime in on wtf the player just saw

		tutorialManager = manager;

		dialogue = new string[2] {
			"Oh my, ",
			""
		};
		yield return tutorialManager.StartDialogue(dialogue);

		// yadda yadda, getting rid of these will earn you the third star


		yield return null;
	}
}
