using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class PointGlove : MonoBehaviour {
	[SerializeField] private List<Vector3> pointGloveLocations;

	private Image image;

	private void Awake() {
		image = GetComponent<Image>();
	}

	public void SetPosition(Vector3 screenPosition) {
		image.rectTransform.localPosition = screenPosition;

	}
}
