using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarDisplay : MonoBehaviour {
	[SerializeField] private Image leftStarImage;
	[SerializeField] private Image middleStarImage;
	[SerializeField] private Image rightStarImage;

	public void SetImage(Sprite starImage) {
		leftStarImage.sprite = starImage;
		middleStarImage.sprite = starImage;
		rightStarImage.sprite = starImage;
	}

	public void UpdateDisplay(bool[] data) {
		leftStarImage.gameObject.SetActive(data[0]);
		middleStarImage.gameObject.SetActive(data[1]);
		rightStarImage.gameObject.SetActive(data[2]);
	}
}
