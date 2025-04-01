using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChipStackInfo : MonoBehaviour {
	[SerializeField] private TextMeshPro chipMainCounterText;
	[SerializeField] private TextMeshPro chipSubCounterText;
	[SerializeField] private AnimationCurve textScaleCurve;

	private RectTransform rect;
	private Vector3 defaultScale;
	private float baseTextHeight;

	private const float NUMBER_TWEEN_DURATION = 0.2f;

	private void Awake() {
		rect = chipMainCounterText.GetComponent<RectTransform>();
		defaultScale = transform.localScale;
		baseTextHeight = rect.position.y;
	}

	public void UpdateCounter(int mainAmount, int subAmount, float offsetValue) {
		if (mainAmount == 0) chipMainCounterText.text = "";
		else chipMainCounterText.text = mainAmount.ToString();

		if (subAmount == 0) chipSubCounterText.text = "";
		else chipSubCounterText.text = subAmount.ToString();
		rect.localPosition = new Vector3(0, baseTextHeight + (mainAmount + subAmount) * offsetValue);
	}

	public void PlayStackCompletionAnimation() {
		StartCoroutine(CompletionAnimation());
	}

	public IEnumerator CompletionAnimation() {
		transform.localScale = defaultScale;
		yield return TweenCounterSize(1f);
	}

	private IEnumerator TweenCounterSize(float sizeIncrease) {
		Vector3 startScale = transform.localScale;
		float elapsedTime = 0f;
		float scaleValue = 1f;

		// scale up
		while (elapsedTime < NUMBER_TWEEN_DURATION) {
			scaleValue = 1f + sizeIncrease * textScaleCurve.Evaluate(elapsedTime / NUMBER_TWEEN_DURATION);
			transform.localScale = startScale * scaleValue;
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		float endScaleValue = scaleValue;
		elapsedTime = 0f;
		while (elapsedTime < NUMBER_TWEEN_DURATION) {
			scaleValue = endScaleValue - (sizeIncrease / 2) * textScaleCurve.Evaluate(elapsedTime / NUMBER_TWEEN_DURATION);
			transform.localScale = startScale * scaleValue;
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}
}
