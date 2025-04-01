using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SMG_UiAngel : MonoBehaviour { // script for a common ui image element behaviour i've dubbed an "angel" it spawns, moves up at a set pace then disappears after a short while
	[SerializeField] private Image image;
	[Range(100f, 1000f)][SerializeField] private float animSpeed;
	[Range(0.1f, 2f)][SerializeField] private float animDuration;

	private RectTransform rect;
	private Vector3 startPosition;
	private Vector3 movementStageVector;
	private float timerValue;
	private bool animating;

	private void Awake() {
		rect = GetComponent<RectTransform>();
	}

	public void StartAnimating(Vector3 startPosition, Sprite image, float speed) {
		this.startPosition = new Vector3(startPosition.x - image.rect.width, startPosition.y, 0);
		movementStageVector = Vector3.zero;
		SetImage(image);
		SetSpeed(speed);
		animating = true;
		timerValue = animDuration;
	}

	private void Update() {
		if (animating) {
			if (timerValue < 0) Disappear();
			movementStageVector += Vector3.up * animSpeed * Time.deltaTime;
			rect.position = startPosition + movementStageVector;
			timerValue -= Time.deltaTime;
		}
	}

	private void Disappear() {
		animating = false;
		image.sprite = null;
		gameObject.SetActive(false);
	}

	public void SetImage(Sprite sprite) {
		image.sprite = sprite;
	}

	public void SetSpeed(float value) {
		animSpeed = value;
	}
}
