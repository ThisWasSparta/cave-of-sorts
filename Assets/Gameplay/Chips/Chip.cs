using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour {
	[SerializeField] private AnimationCurve yCurve;
	[SerializeField] private float animationDuration;
	[SerializeField] private float disappearAnimationHeight;
	[SerializeField] private ChipType type;

	private ChipStack ownerStack;

	public ChipStack OwnerStack { get { return ownerStack; } }
	public ChipType Type { get { return type; } }

	private void Start() {
		animationDuration = 0.4f;
		disappearAnimationHeight = 1f;
	}

	public IEnumerator AnimateTo(BoardChipStack destination) {
		int tileAmount = destination.ChipAmount;
		destination.AddChip(this);
		AssignOwner(destination);
		Vector3 startVector = transform.position;
		Vector3 destinationVector = new Vector3(destination.transform.position.x, tileAmount * destination.InbetweenTileHeight, destination.transform.position.z);
		float animTimer = 0f;

		while (animTimer <= animationDuration) {
			float animPercentage = animTimer / animationDuration;
			float xValue = Mathf.Lerp(startVector.x, destinationVector.x, animPercentage);
			float yValue = Mathf.Lerp(startVector.y, destinationVector.y, animPercentage);
			float zValue = Mathf.Lerp(startVector.z, destinationVector.z, animPercentage);
			transform.position = new Vector3(xValue, yValue, zValue);
			animTimer += Time.deltaTime;
			yield return null;
		}
		transform.position = destinationVector; // explicitly assign position afterward to account for any possible lag
	}

	public IEnumerator AnimateAway() {
		float startY = transform.localPosition.y;
		float animTimer = 0f;

		while (animTimer <= animationDuration) {
			float animPercentage = animTimer / animationDuration;
			float yValue = Mathf.Lerp(startY, startY + disappearAnimationHeight, animPercentage);
			float scaleValue = Mathf.Lerp(1f, 0f, animPercentage);
			transform.localPosition = new Vector3(transform.localPosition.x, yValue, transform.localPosition.z);
			transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
			animTimer += Time.deltaTime;
			yield return null;
		}

		ReturnToPool();
		yield return null;
	}

	public void AssignOwner(ChipStack stack) {
		transform.SetParent(stack.transform, true);
		ownerStack = stack;
	}

	public void UpdatePosition() {
		this.transform.localPosition = new Vector3(0f, ownerStack.ChipAmount * ownerStack.InbetweenTileHeight, 0f);
	}

	public void UpdatePosition(int index) {
		this.transform.localPosition = new Vector3(0f, index * ownerStack.InbetweenTileHeight, 0f);
	}

	private void ReturnToPool() {
		this.ownerStack = null;
		this.transform.SetParent(null);
		this.transform.localScale = Vector3.one;
		this.transform.position = Vector3.zero;
		this.gameObject.SetActive(false);
	}
}

public enum ChipType {
	Placeholder,
	Red,
	Yellow,
	Green,
	Blue,
	Purple,
	White,
	Black,
	Brown,
	Pink,
	WildCard,
	Invalid
}
