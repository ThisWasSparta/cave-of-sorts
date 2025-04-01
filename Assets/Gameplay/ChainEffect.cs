using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEffect : MonoBehaviour {
	[SerializeField] protected SpriteRenderer topChain;
	[SerializeField] protected List<SpriteRenderer> sideChains;
	[SerializeField] protected float inAnimationDuration;
	[SerializeField] protected float outAnimationDuration;
	[SerializeField] protected float jiggleAnimationDuration;
	[SerializeField] protected float degreesRotation = 30f;

	[SerializeField] protected AnimationCurve jiggleCurve; // smth back and forth around 0 like -/\/-

	protected IEnumerator currentAnimation;
	protected bool animating;
	protected float chainWidth;
	protected float topChainEndSize;
	protected float sideChainEndSize;

	protected void Start() {
		chainWidth = topChain.size.x;
		topChainEndSize = topChain.size.y;
		sideChainEndSize = sideChains[0].size.y;
		ResetChainSize();
	}

	public void AnimateIn() {
		if (animating) StopCoroutine(currentAnimation);
		currentAnimation = StartInAnimation();
		StartCoroutine(currentAnimation);
	}

	public virtual void AnimateOut() {
		if (animating) StopCoroutine(currentAnimation);
		currentAnimation = StartOutAnimation();
		StartCoroutine(currentAnimation);
	}

	public void PlayEmphasisAnimation() { // used for the exit as well as tiles
		if (animating) return;
		currentAnimation = EmphasisAnimation();
		StartCoroutine(currentAnimation);
	}

	protected IEnumerator StartInAnimation() {
		animating = true;
		var counter = 0f;
		ResetChainSize();

		while (counter < inAnimationDuration) {
			counter += Time.deltaTime;
			topChain.size = new Vector2(chainWidth, counter / inAnimationDuration * topChainEndSize);
			for (int i = 0; i < sideChains.Count; i++) {
				sideChains[i].size = new Vector2(chainWidth, counter / inAnimationDuration * sideChainEndSize);
			}
			yield return null;
		}
		animating = false;
	}

	public IEnumerator StartOutAnimation() {
		animating = true;
		var counter = outAnimationDuration;

		while (counter > 0) {
			counter -= Time.deltaTime;
			topChain.size = new Vector2(chainWidth, counter / outAnimationDuration * topChainEndSize);
			for (int i = 0; i < sideChains.Count; i++) {
				sideChains[i].size = new Vector2(chainWidth, counter / outAnimationDuration * sideChainEndSize);
			}
			yield return null;
		}
		ResetChainSize(); // in case values are over-/undershot reset to start positions
		animating = false;
	}

	public IEnumerator EmphasisAnimation() {
		animating = true;
		JiggleChain(0);
		yield return new WaitForSeconds(0.1f);
		JiggleChain(1);
		yield return new WaitForSeconds(0.1f);
		yield return JiggleChain(2);
		animating = false;
	}

	public IEnumerator JiggleChain(int index) { // !! UNTESTED !!
		SpriteRenderer chain;
		if (index == 0) {
			chain = topChain;
		} else {
			chain = sideChains[index - 1];
		}

		// SoundManager.Instance.PlaySound("chainsfx.wav"); TODO: create sound tutorialManager

		float counter = jiggleAnimationDuration;
		float progress = 0;
		Quaternion identity = chain.transform.rotation;
		while (counter > 0) { // smaller / greater
			progress = counter / jiggleAnimationDuration;
			chain.transform.rotation = identity * Quaternion.AngleAxis(jiggleCurve.Evaluate(progress) * degreesRotation, Vector3.forward); // check if angleaxis actually works like this
			// axis to rotate image without rotating away from camera is... z? verify plz
		}

		yield return null;
	}

	protected void ResetChainSize() {
		var startSize = new Vector2(chainWidth, 0f);
		topChain.size = startSize;
		for (int i = 0; i < sideChains.Count; i++) {
			sideChains[i].size = startSize;
		}
	}
}
