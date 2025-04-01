using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour {
	public delegate void ComboEvent(int comboLevel);
	public static event ComboEvent onComboPayout;

	[SerializeField] private List<Sprite> comboCountImages;
	[SerializeField] private GameObject angelObject;
	[SerializeField] private RectTransform canvasRoot;

	[SerializeField] private ObjectiveTracker objectiveTracker;

	[SerializeField] private float angelAnimationSpeed;

	private Camera mainCam;
	private SMG_ObjectPool angelPool;
	private int currentClearCombo;

	private void Start() {
		mainCam = Camera.main;
		angelPool = new SMG_ObjectPool(angelObject, canvasRoot, 8);
	}

	public void IncreaseClearCombo(Vector3 worldPosition) {
		currentClearCombo++;
		var screenPos = mainCam.WorldToScreenPoint(worldPosition);
		var angelObj = angelPool.GetPooledObject();
		angelObj.SetActive(true);
		int comboIndex = currentClearCombo >= comboCountImages.Count ? comboCountImages.Count : currentClearCombo;
		angelObj.GetComponent<SMG_UiAngel>().StartAnimating(screenPos, comboCountImages[comboIndex - 1], angelAnimationSpeed);
	}

	public void ResetAndPayout(bool matchesFound) {
		if (currentClearCombo > 0) objectiveTracker.AddToCombo(currentClearCombo);
		// if (currentClearCombo > 0) onComboPayout?.Invoke(currentClearCombo);
		currentClearCombo = 0;
	}

	private void OnEnable() {
		MatchResolver.BoardEvaluated += ResetAndPayout;
	}

	private void OnDisable() {
		MatchResolver.BoardEvaluated -= ResetAndPayout;
	}
}
