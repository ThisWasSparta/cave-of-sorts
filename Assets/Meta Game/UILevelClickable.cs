using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILevelClickable : MonoBehaviour {
	public delegate void ClickableEvent();
	public static event ClickableEvent OnInteract;

	[SerializeField] private TextMeshProUGUI indexTextObject;
	[SerializeField] private StarDisplay starDisplay;

	private bool[] starData;
	private string levelKey;
	private int levelIndex;
	private bool cleared;

	public bool Cleared { get { return cleared; } }

	public void SetIndexValue(int worldIndex, int levelIndex, Sprite starSprite) {
		this.levelIndex = levelIndex;
		indexTextObject.text = (this.levelIndex + 1).ToString();

		levelKey = $"{worldIndex}{levelIndex}";
		if (!GameManager.Instance.SaveDataManager.TryGetStars(levelKey, out starData)) {
			starData = new bool[3];
		}
		starDisplay.SetImage(starSprite);
		starDisplay.UpdateDisplay(starData);
		if (starData[1] == true) cleared = true;
	}

	public void OnClick() {
		WorldScreenManager.Instance.OnLevelSelect(levelKey, levelIndex);
		OnInteract?.Invoke();
	}
}
