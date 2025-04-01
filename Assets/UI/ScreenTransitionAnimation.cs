using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransitionAnimation : MonoBehaviour { // not very generic, but wtf ever at this point
	[SerializeField] private Canvas canvas;
	[SerializeField] private AnimationCurve curtainSpeedCurve;
	[SerializeField] private float animationDuration;
	[SerializeField] private float animationPacingDelayDuration;

	[SerializeField] private Image bottomToTopImage;
	[SerializeField] private Image topToBottomImage;
	[SerializeField] private Image screenCover;

	private RectTransform canvasRect;
	private RectTransform bttRect;
	private RectTransform ttbRect;

	private WaitForSeconds animationPacingDelay;

	private const int CURTAIN_START_HEIGHT = 64; // slight height just ot make it visible in the inspector when idle, higher than this makes it peak out of the top/bottom
	private const int CURTAIN_BUFFER = 96; // curtain image is offscreen slightly/needs to cover for its transparency to cover the whole screen

	private void Start() {
		canvasRect = canvas.GetComponent<RectTransform>();
		bttRect = bottomToTopImage.GetComponent<RectTransform>();
		ttbRect = topToBottomImage.GetComponent<RectTransform>();
		animationPacingDelay = new WaitForSeconds(animationPacingDelayDuration);
	}

	public IEnumerator PlayAnimation(bool bottomToTop, bool animateInward) {
		RectTransform curtainToAnimate;
		RectTransform otherCurtain;
		canvasRect = canvas.GetComponent<RectTransform>();
		float goalValue = canvasRect.rect.height + CURTAIN_BUFFER;

		if (bottomToTop && animateInward || !bottomToTop && !animateInward) { //outward animation is played by other side to keep images in place
			curtainToAnimate = bttRect;
			otherCurtain = ttbRect;
		}
		else {
			curtainToAnimate = ttbRect;
			otherCurtain = bttRect;
		}

		if (!animateInward) {
			curtainToAnimate.sizeDelta = new Vector2(curtainToAnimate.sizeDelta.x, goalValue);
			otherCurtain.sizeDelta = new Vector2(otherCurtain.sizeDelta.x, CURTAIN_START_HEIGHT);
		}

		float counter = 0;
		while (counter < animationDuration) {
			float normalized = counter / animationDuration;
			float currentHeight = goalValue * curtainSpeedCurve.Evaluate(normalized);
			if (!animateInward) currentHeight = goalValue - currentHeight;
			curtainToAnimate.sizeDelta = new Vector2(curtainToAnimate.sizeDelta.x, currentHeight);

			counter += Time.deltaTime;
			yield return null;
		}

		yield return animationPacingDelay;
	}

	public void CoverScreen(bool cover) {
		screenCover.gameObject.SetActive(cover);
	}
}
