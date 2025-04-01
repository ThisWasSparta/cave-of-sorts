using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween : MonoBehaviour { // ! UNTESTED !
	[Header(header: "Parameters")]
	[SerializeField] private GameObject subject;
	[SerializeField] private AnimationType animationType;

	[SerializeField] private float durationInSeconds = 1.0f;

	public Vector3 from;
	public Vector3 to;

	public AnimationCurve xBoomerangCurve; // keep in mind, future me, these have yet to be made!
	public AnimationCurve yBoomerangCurve; 

	public void SetupTween(Vector2 from, Vector2 to, float duration, out IEnumerator tweenRoutine) {
		durationInSeconds = duration;
		tweenRoutine = PlayTween(from, to);
		
	}

	public void SetupTween(Vector2 from, Vector2 to, out IEnumerator tweenRoutine) {
		tweenRoutine = PlayTween(from, to);
	}

	private IEnumerator PlayTween(Vector3 from, Vector3 to) {
		float timer = durationInSeconds;
		int haywire = 0;

		while (timer > 0) {
			if (haywire > 90000) {
				Debug.LogWarning("while loop has gone haywire! Abort! Abort!");
				break;
			}
			haywire++;
			timer -= Time.deltaTime;

			Vector3 currentPosition;
			currentPosition = Vector3.Lerp(from, to, timer / durationInSeconds); // if in a straight line
			float xDistance = to.x - from.x;
			float yDistance = to.y - from.y;
			float currentX = currentPosition.x;
			float currentY = currentPosition.y;

			switch (animationType) {
				case AnimationType.Boomerang:
					currentX = xDistance * xBoomerangCurve.Evaluate(timer / durationInSeconds);
					currentY = yDistance * yBoomerangCurve.Evaluate(timer / durationInSeconds);
					break;
				case AnimationType.BoomerangHorizontal:
					currentX = xDistance * xBoomerangCurve.Evaluate(timer / durationInSeconds);
					break;
				case AnimationType.BoomerangVertical:
					currentY = yDistance * yBoomerangCurve.Evaluate(timer / durationInSeconds);
					break;
				default:
					break;
			}

			currentPosition = new Vector3(currentX, currentY, 0f);
			subject.transform.position = currentPosition;
			yield return null;
		}
	}
}

public enum AnimationType {
	StraightLine, // move towards goal x and y without funny business
	Boomerang, // move towards negative before curving back towards goal
	BoomerangHorizontal, // move y straight, boomerang x
	BoomerangVertical // move x straight, boomering y
}
