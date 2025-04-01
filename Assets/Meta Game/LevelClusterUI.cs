using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelClusterUI : UIScreenContainer {
	[SerializeField] private List<UILevelClickable> levelButtons;

	private LevelClusterWorld worldObject;
	private Camera mainCam;

	public void Setup(LevelClusterWorld worldObject, Vector3 worldPosition, int worldIndex, int clusterIndex, Sprite starSprite) {
		this.worldObject = worldObject;
		mainCam = Camera.main;

		transform.position = mainCam.WorldToScreenPoint(worldPosition);

		int levelIndex = clusterIndex * 5; // this needs to know which of the four clusters it is, now what world it's in, silly billy
		foreach (var clickable in levelButtons) {
			clickable.SetIndexValue(worldIndex, levelIndex, starSprite);
			levelIndex++;
		}
	}

	public bool ClearedAll() {
		foreach (var button in levelButtons) {
			if (!button.Cleared) return false;
		}
		return true;
	}

	public bool IsCompleted() {
		foreach (var button in levelButtons) {
			if (!button.Cleared)
				return false;
		}

		return true;
	}

	public void Show() {
		screenContainer.SetActive(true);
	}

	public void Hide() {
		screenContainer.SetActive(false);
	}

	private void OnEnable() {
		UILevelClickable.OnInteract += Hide;
	}

	private void OnDisable() {
		UILevelClickable.OnInteract -= Hide;
	}
}
