using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectScreen : UIScreenContainer {
	[SerializeField] private TextMeshProUGUI headerText;
	[SerializeField] private StarDisplay starDisplay;

	private LevelDataContainer dataContainer;
	private Sprite starSprite;

	private bool[] starData;
	private string levelKey;

	public void EnableAndPopulate(int worldIndex, int levelIndex, string levelKey, LevelDataContainer data, bool[] starData, Sprite starSprite) {
		this.levelKey = levelKey;
		dataContainer = data;
		this.starData = starData;
		this.starSprite = starSprite;
		headerText.text = $"{worldIndex + 1}-{levelIndex + 1}";
		starDisplay.UpdateDisplay(starData);
		starDisplay.SetImage(starSprite);
		ShowScreen();
	}

	public void OnStartButtonPress() {
		GameManager.Instance.TransitionToAndLoadLevel(dataContainer.contentData, dataContainer.gridData, levelKey, dataContainer.dealerLevel, starData, starSprite);
	}

	public void OnBackButtonPress() {
		WorldScreenManager.Instance.ReenableControl();
		HideScreen();
	}
}
