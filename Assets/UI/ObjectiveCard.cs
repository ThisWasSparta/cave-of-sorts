using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveCard : MonoBehaviour {
	[SerializeField] private Image contentImage;
	[SerializeField] private Image chipGoldTrimImage;
	[SerializeField] private Image completionTickImage;
	[SerializeField] private TextMeshProUGUI counterText;

	public bool IsCompleted { get { return currentCount >= goalCount; } }

	private int currentCount;
	private int goalCount;

	public void SetImage(Sprite imageToSet, Color colour) {
		contentImage.sprite = imageToSet;
		contentImage.color = colour;
	}

	public void SetCounterGoal(int goal) {
		goalCount = goal;
	}

	public void AddToCounterAndUpdateText(int count) {
		if (IsCompleted) return;
		currentCount += count;
		counterText.text = $"{currentCount}/{goalCount}";
		if (IsCompleted) {
			counterText.gameObject.SetActive(false);
			completionTickImage.gameObject.SetActive(true);
		}
	}
}
