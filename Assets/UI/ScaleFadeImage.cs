using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleFadeImage : MonoBehaviour { // courtesy of my good friend chat gpt because frankly fuck making these myself
	[SerializeField] private AnimationCurve scaleCurve;
	[SerializeField] private AnimationCurve fadeCurve;

	[SerializeField] private Image image;
	[SerializeField] private float duration;

	private void Awake() {
		StartCoroutine(ScaleAndFade());
	}

	private IEnumerator ScaleAndFade() {
		Vector3 originalScale = image.rectTransform.localScale;
		Color originalColor = image.color;

		while (true) {
			for (float t = 0; t < duration; t += Time.deltaTime) {
				float normalizedTime = t / duration;

				float scaleValue = scaleCurve.Evaluate(normalizedTime);
				image.rectTransform.localScale = originalScale * scaleValue;

				float fadeValue = fadeCurve.Evaluate(normalizedTime);
				image.color = new Color(originalColor.r, originalColor.g, originalColor.b, fadeValue);

				yield return null;
			}
		}
	}
}
