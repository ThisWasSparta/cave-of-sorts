using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScreenInputSystem : MonoBehaviour { // separate control systems per screen is perhaps a bit jank, but keeps logic separate
	private List<LevelClusterWorld> clusterObjects;
	private WorldExit exitObject;
	private Camera mainCam;

	private RaycastHit raycastHit;
	private bool activelyListening = true;

	private void Awake() {
		mainCam = Camera.main;
	}

	private void Update() {
		if (!activelyListening) return;
	
		if (Input.GetMouseButtonDown(0)) {
			var mousePos = Input.mousePosition;
			Ray ray = mainCam.ScreenPointToRay(mousePos);

			Physics.Raycast(ray, out raycastHit, Mathf.Infinity);
			InteractableObject interactable;
			if (raycastHit.collider != null && raycastHit.collider.gameObject.TryGetComponent(out interactable)) {
				(interactable as IInteractable).OnInteract();
			}
		}
	}

	public void SetListeningState(bool setActive) {
		activelyListening = setActive;
	}
}
