using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenBehaviour : UIScreenContainer {
	[SerializeField] private Button pauseButton;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button exitLevelButton;
	[SerializeField] private Button continueLevelButton;

	[SerializeField] private GameplayInputSystem inputSystem;

	public override void ShowScreen() {
		base.ShowScreen();
		inputSystem.SetControlState(false); // inputsystem can be referenced through public field in grid tutorialManager, maybe do that instead
	}

	public override void HideScreen() {
		base.HideScreen();
		inputSystem.SetControlState(true);
	}

	public void OnRestart() {
		GameManager.Instance.RestartLevel();
	}

	public void OnBackToMenu() {
		GameManager.Instance.BackToWorldMap();
	}

	public void SetPauseButtonState(bool setActive) {
		pauseButton.interactable = setActive;
	}
}
