using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragGlove : MonoBehaviour {
	[SerializeField] private AnimationCurve moveCurve;
	[SerializeField] private AnimationCurve squashCurve;
	[SerializeField] private Sprite[] sprites;
	[SerializeField] private Image pointGlove;
	[SerializeField] private float moveDuration = 1.3f;
	[SerializeField] private float squashDuration = 0.2f;
	[SerializeField] private float pacingWaitDuration = 0.3f;
	[SerializeField] private float inputWaitDuration = 0.15f;

	private Camera mainCam;
	private Image image;
	private WaitForSeconds pacingWait;
	private WaitForSeconds inputWait;
	private IEnumerator animationRoutine;
	private bool currentlyAnimating;

	public bool Animating { get { return currentlyAnimating; } }

	private void Start() {
		image = GetComponent<Image>();
		mainCam = Camera.main;
		pacingWait = new WaitForSeconds(pacingWaitDuration);
		inputWait = new WaitForSeconds(inputWaitDuration);
		gameObject.SetActive(false);
		pointGlove.gameObject.SetActive(false);
	}

	public void PlayAnimation(Vector3 from, Vector3 to) {
		image.rectTransform.position = from;
		animationRoutine = RepeatAnimation(from, to);
		currentlyAnimating = true;
		StartCoroutine(animationRoutine);
	}

	public IEnumerator RepeatAnimation(Vector3 from, Vector3 to) { 
		while (true) {
			yield return Animate(from, to, true);
			yield return Animate(from, to, false);
		}
	}

	public IEnumerator Animate(Vector3 from, Vector3 to, bool tap) {
		yield return AnimateTapOrHold(tap);
		yield return AnimateMove(from, to);

		if (tap) {
			yield return AnimateTapOrHold(tap);
		}
		else {
			image.sprite = sprites[0];
			yield return pacingWait;
		}

		yield return pacingWait;
		yield return AnimateMove(to, from);
		yield return pacingWait;
	}

	private IEnumerator AnimateTapOrHold(bool tap) {
		if (tap) StartCoroutine(SquashSprite());
		image.sprite = sprites[1];
		yield return inputWait;
		if (tap) image.sprite = sprites[0];
	}

	private IEnumerator AnimateMove(Vector3 from, Vector3 to) {
		float timer = moveDuration;
		Vector3 fromScreen = mainCam.WorldToScreenPoint(from);
		Vector3 toScreen = mainCam.WorldToScreenPoint(to);

		while (timer > 0) {
			timer -= Time.deltaTime;
			image.rectTransform.position = Vector3.Lerp(fromScreen, toScreen, moveCurve.Evaluate(1 - timer / moveDuration));
			yield return null;
		}
	}

	private IEnumerator SquashSprite() {
		float timer = squashDuration;
		float startHeight = image.rectTransform.sizeDelta.y;

		while (timer > 0) {
			timer -= Time.deltaTime;
			image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, startHeight * squashCurve.Evaluate(1 - timer / squashDuration));
			yield return null;
		}
	}

	public void StopAnimating() {
		if (!currentlyAnimating) return;
		StopCoroutine(animationRoutine);
	}

	private void OnDisable() {
		if (currentlyAnimating) StopCoroutine(animationRoutine);
		currentlyAnimating = false;
	}
}
