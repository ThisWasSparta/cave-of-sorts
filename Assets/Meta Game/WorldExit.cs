using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldExit : InteractableObject, IInteractable {
	[SerializeField] private ChainEffect chainEffect;
	[SerializeField] private UIStarCounter starCounter;

	private WorldLayoutObject parentWorld;
	private bool locked;

	public bool Locked { get { return locked; } }

	private void Start() {
		if (locked) chainEffect.AnimateIn();
		else chainEffect.AnimateOut();
	}

	public override void OnInteract() {
		if (parentWorld.StarQuotaReached()) {
			if (locked) {
				PlayUnlockExitAnimation();
			}
			else {
				WorldScreenManager.Instance.LoadNextWorld();
			}
		}
		else {
			PlayLockedAnimation();
		}
	}

	public void PlayUnlockExitAnimation() {
		IEnumerator coroutine = UnlockAnimation();
		StartCoroutine(coroutine);
	}

	private IEnumerator UnlockAnimation() {
		// light, beams around it or smth that spin around
		// chains glow white before retreating
		// play sfx? whatever, you'll figure smth out

		// SoundManager.Instance.DoSomething("plz");

		chainEffect.AnimateOut();
		yield return null;
	}

	private void PlayLockedAnimation() {

		// play chain jiggle sfx
		// tween star counter?
		starCounter.PlayEmphasisAnimation();
		chainEffect.PlayEmphasisAnimation();
	}

	public void OnWorldLoad(WorldLayoutObject parentWorld, bool locked, int worldStarQuota) {
		chainEffect.AnimateIn();
		this.parentWorld = parentWorld;
		this.locked = locked;
		starCounter.SetCounterValue(worldStarQuota);
	}
}
