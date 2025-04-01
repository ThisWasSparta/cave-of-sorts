using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenContainer : MonoBehaviour {
	[SerializeField] protected GameObject screenContainer;
	[SerializeField] protected bool startOpen;

	public virtual void Awake() {
		if (startOpen) screenContainer.SetActive(true);
		else screenContainer.SetActive(false);
	}

	public virtual void ShowScreen() {
		screenContainer.SetActive(true);
	}

	public virtual void HideScreen() {
		screenContainer.SetActive(false);
	}
}
