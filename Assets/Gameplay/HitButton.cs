using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitButton : MonoBehaviour {
	[SerializeField] private Color disabledButtonColour;

	private Button buttonComponent;

	public Button Button { get { return this.buttonComponent; } }

	private void Awake() {
		buttonComponent = GetComponent<Button>();
	}

	public void SetButtonActive(bool active) {
		if (active) {

			buttonComponent.image.color = Color.white;
			buttonComponent.interactable = true;
		}
		else {
			buttonComponent.image.color = disabledButtonColour;
			buttonComponent.interactable = false;
		}
	}
}
