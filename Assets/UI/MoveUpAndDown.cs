using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveUpAndDown : MonoBehaviour { // courtesy of my good friend chat gpt because frankly fuck making these myself
	[SerializeField] private Image targetImage;
	[SerializeField] private float duration = 5.0f;
	[SerializeField] private float multiplier = 1.0f;
	[SerializeField] private AnimationCurve movementCurve;

	void Start() {
		StartCoroutine(MoveUpDownCoroutine(targetImage, duration));
	}

	private IEnumerator MoveUpDownCoroutine(Image image, float duration) {
		Vector3 originalPosition = image.rectTransform.anchoredPosition;

		while (true) {
			for (float t = 0; t < duration; t += Time.deltaTime) {
				float normalizedTime = t / duration;
				float yOffset = movementCurve.Evaluate(normalizedTime) * multiplier;
				image.rectTransform.anchoredPosition = new Vector3(originalPosition.x, originalPosition.y + yOffset, originalPosition.z);

				yield return null;
			}
		}
	}
}
