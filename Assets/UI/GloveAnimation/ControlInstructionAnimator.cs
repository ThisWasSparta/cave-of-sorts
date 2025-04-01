using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlInstructionAnimator : MonoBehaviour {
	[SerializeField] private PointGlove pointGlove;
	[SerializeField] private DragGlove dragGlove;

	[SerializeField] private List<Vector3> pointGloveLocations;

	public PointGlove PointGlove { get { return pointGlove; } }
	public DragGlove DragGlove { get { return dragGlove; } }

	private void Start() {
		pointGlove.gameObject.SetActive(false);
		dragGlove.gameObject.SetActive(false);
	}

	public void PlayPointAnimation(int pos) {
		pointGlove.gameObject.SetActive(true);
		pointGlove.SetPosition(pointGloveLocations[pos]);
	}

	public void PlayDragAnimation(Vector3 from, Vector3 to) {
		dragGlove.gameObject.SetActive(true);
		dragGlove.PlayAnimation(from, to);
	}

	public void StopAnimating() {
		pointGlove.gameObject.SetActive(false);
		dragGlove.gameObject.SetActive(false);
	}
}
