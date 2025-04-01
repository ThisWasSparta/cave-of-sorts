using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class StackChainEffect : ChainEffect {
	public override void AnimateOut() {
		print("animate out");
		base.AnimateOut();
		GridManager.Instance.RegisterChainRelease();
	}
}
