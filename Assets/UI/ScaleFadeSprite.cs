using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFadeSprite : MonoBehaviour {
	[SerializeField] private SpriteRenderer sprite;
	[SerializeField] private AnimationCurve scaleCurve;
	[SerializeField] private AnimationCurve fadeCurve;
	[SerializeField] private float duration = 1.0f;

	private Coroutine scaleAndFadeRoutine;

	private IEnumerator ScaleAndFadeCoroutine() {
		Vector3 originalScale = sprite.transform.localScale;
		Color originalColor = sprite.color;

		while (true) {
			for (float t = 0; t < duration; t += Time.deltaTime) {
				float normalizedTime = t / duration;

				float scaleValue = scaleCurve.Evaluate(normalizedTime);
				sprite.transform.localScale = originalScale * scaleValue;

				float fadeValue = fadeCurve.Evaluate(normalizedTime);
				sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, fadeValue);

				yield return null;
			}
		}
	}

	private void OnDisable() {
		StopCoroutine(scaleAndFadeRoutine);
	}

	private void OnEnable() {
		scaleAndFadeRoutine = StartCoroutine(ScaleAndFadeCoroutine());
	}
}
