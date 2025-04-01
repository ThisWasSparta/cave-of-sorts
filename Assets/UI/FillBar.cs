using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour {
	[SerializeField] private AnimationCurve fillMoveCurve;
	[SerializeField] private Image barFillImage;
	[SerializeField] private Color baseColour;
	[SerializeField] private Color goalColour;
	[SerializeField] private float fillAnimationDuration = 0.1f;
	[SerializeField] private float emptyFillValue = 13f;
	[SerializeField] private float fullFillValue = 100f;

	private RectTransform rect;
	private IEnumerator pulseAnimation;
	private int maxIncrementCount;
	private int currentIncrementCount;
	private float fillPerIncrement;

	private const float SIN_SPEED = 2.5f;

	private void Awake() {
		rect = barFillImage.GetComponent<RectTransform>();
	}

	public void Setup(int increments) {
		maxIncrementCount = increments;
		fillPerIncrement = (fullFillValue - emptyFillValue) / maxIncrementCount;
		rect.sizeDelta = new Vector2(emptyFillValue, 100f);
	}

	public void AddIncrements(int amountToAdd, out bool full) {
		full = false;
		currentIncrementCount += amountToAdd;
		barFillImage.rectTransform.sizeDelta = new Vector2(Mathf.Clamp(emptyFillValue + fillPerIncrement * currentIncrementCount, emptyFillValue, fullFillValue), fullFillValue);
		if (currentIncrementCount >= maxIncrementCount) full = true;
	}

	private IEnumerator AnimateIncrease() { // TODO
		while (true) {
			yield return null;
		}
	}

	private IEnumerator AnimatePulse() {
		while (true) {
			barFillImage.color = Color.Lerp(baseColour, goalColour, (Mathf.Sin(Time.time * SIN_SPEED) + 1) / 2);
			yield return null;
		}
	}

	private void OnEnable() {
		pulseAnimation = AnimatePulse();
		StartCoroutine(pulseAnimation);
	}

	private void OnDisable() {
		StopAllCoroutines();
	}
}
