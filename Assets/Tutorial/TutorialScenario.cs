using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialScenario : ScriptableObject { // class that defines the behaviour in individual "scenarios" in the tutorial
	protected TutorialManager tutorialManager;

	public LevelGridData gridData;
	public Vector3Int[] allowedPlacementCoords;
	[HideInInspector] public string[] dialogue;
	[HideInInspector] public bool placeFlag;
	[HideInInspector] public bool matchFlag;

	public abstract IEnumerator ProcessScenario(TutorialManager manager);

	protected void OnValidTilePlacement(Vector3Int position) {
		placeFlag = true;
	}

	protected void OnMatchLogicFinish() {
		matchFlag = true;
	}

	private void OnEnable() {
		AnimationCoordinator.MatchingConcluded += OnMatchLogicFinish;
		GameplayInputSystem.ValidPositionFound += OnValidTilePlacement;
	}

	private void OnDisable() {
		AnimationCoordinator.MatchingConcluded -= OnMatchLogicFinish;
		GameplayInputSystem.ValidPositionFound -= OnValidTilePlacement;
	}
}
