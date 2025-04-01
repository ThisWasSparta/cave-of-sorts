using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelClusterWorld : InteractableObject, IInteractable {
	public delegate void LevelClusterEvent();
	public static event LevelClusterEvent OnClusterShow;

	[SerializeField] private Transform uiOrigin;
	[SerializeField] private GameObject lanternGlow;

	private LevelClusterUI uiObject;
	private int clusterIndex;
	private bool shown;

	public int Index { get { return clusterIndex; } }
	
	public void AssignCluster(LevelClusterUI clusterObj, int worldIndex, int clusterIndex, Sprite starSprite) {
		uiObject = clusterObj;
		clusterObj.Setup(this, uiOrigin.position, worldIndex, clusterIndex, starSprite);
		if (uiObject.IsCompleted() || clusterIndex == 0) lanternGlow.SetActive(true);
		else lanternGlow.SetActive(false);
	}

	public override void OnInteract() {
		if (shown) {
			HideClusterUi();
			return;
		}
		OnClusterShow();
		uiObject.Show();
		shown = true;
	}

	public void HideClusterUi() {
		uiObject.Hide();
		shown = false;
	}

	public bool IsCleared() {
		return uiObject.ClearedAll();
	}

	private void HandleInput() {
		if (shown) HideClusterUi();
	}

	private void OnEnable() {
		OnClusterShow += HandleInput;
	}

	private void OnDisable() {
		OnClusterShow -= HandleInput;
	}
}
