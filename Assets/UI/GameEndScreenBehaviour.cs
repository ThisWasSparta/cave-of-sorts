using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndScreenBehaviour : UIScreenContainer {
	[SerializeField] private Image headerImage;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button exitButton;
	[SerializeField] private Button nextButton;
	[SerializeField] private AnimationCurve starAnimCurve;
	[SerializeField] private StarDisplay starDisplay;

	[Header("SPRITES")]
	[SerializeField] private Sprite winnerSprite;
	[SerializeField] private Sprite loserSprite;

	private Sprite starImage;
	private IEnumerator endAnimationRoutine;

	private void Start() {
		starDisplay.UpdateDisplay(new bool[3]);
	}

	public void StartEndAnimation(bool won, bool[] starData, Sprite starImage) {
		if (won) headerImage.sprite = winnerSprite;
		else headerImage.sprite = loserSprite;
		this.starImage = starImage;
		starDisplay.SetImage(starImage);
		if (won) starDisplay.UpdateDisplay(starData);
		endAnimationRoutine = EndAnimationCoroutine(won, starData);
		StartCoroutine(endAnimationRoutine);
	}

	private IEnumerator EndAnimationCoroutine(bool won, bool[] starData) {
		if (won) nextButton.interactable = true;
		yield return null;
	}

	public void OnContinue() {
		GameManager.Instance.StartNextLevel();
	}

	public void OnRestart() {
		GameManager.Instance.RestartLevel();
	}

	public void OnBackToMenu() {
		GameManager.Instance.BackToWorldMap();
	}
}
