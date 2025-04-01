using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStarCounter : MonoBehaviour {
	[SerializeField] private Image backgroundImage;
	[SerializeField] private Image starImage;
	[SerializeField] private TextMeshProUGUI counterText;

	public void SetCounterValue(int value) {
		counterText.text = value.ToString();
	}

	public void SetStarImage(Sprite sprite) {
		starImage.sprite = sprite;
	}

	public void PlayEmphasisAnimation() {
		IEnumerator coroutine = EmphasisAnimation();
		StartCoroutine(coroutine);
	}

	private IEnumerator EmphasisAnimation() {
		// starimage inflates before jiggling? smth like that

		// quick tween the size of the whole thing to signal "yo dipshit, not enough stars yet!"
		yield return null;
	}
}
