using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCoordinator : MonoBehaviour {
	public delegate void CoordinatorEvent();
	public static event CoordinatorEvent MatchingConcluded;

	[Header("Animation duration values")]
	[SerializeField] private GameObject fireUpParticleEffect;
	[SerializeField] private float inbetweenAnimWaitDuration = 0.3f;
	[SerializeField] private float maxMatchAnimationDuration = 0.6f;

	private ComboSystem comboSystem;
	private LinkedList<Vector3Int> stackPositionsToEvaluate;

	private List<BoardChipStack> completedStacks;

	private SMG_ObjectPool particlePool;
	private List<GameObject> particleEffects;

	private HashSet<BoardChipStack> cursedContainer;

	private MatchResolver matchResolver;
	private BoardChipStack currentFrom;
	private BoardChipStack currentTo;

	private WaitForSeconds inbetweenAnimWait;
	private WaitForSeconds beforeCurseWait;
	private WaitForSeconds inbetweenCurseWait;

	private IEnumerator evaluationRoutine;

	private AnimationLink currentLink;

	public int CompletedStacksPresent { get { return completedStacks.Count; } }

	public static AnimationCoordinator Instance;

	private static int PARTICLE_EFFECT_POOL_AMOUNT = 5;
	private static float BEFORE_CURSE = 0.6f;
	private static float BETWEEN_CURSE = 0.01f;

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(this);
			Debug.LogWarning($"{this.name} instance already exists! Destroying duplicate.");
		}
		else {
			Instance = this;
		}

		particlePool = new SMG_ObjectPool(fireUpParticleEffect, this.transform, 6);
	}

	private void Start() {
		comboSystem = GetComponent<ComboSystem>();
		inbetweenAnimWait = new WaitForSeconds(inbetweenAnimWaitDuration);
		beforeCurseWait = new WaitForSeconds(BEFORE_CURSE);
		inbetweenCurseWait = new WaitForSeconds(BETWEEN_CURSE);
		matchResolver = GetComponent<MatchResolver>();
		stackPositionsToEvaluate = new LinkedList<Vector3Int>();
		completedStacks = new List<BoardChipStack>();
		particleEffects = new List<GameObject>();

		for (int i = 0; i < PARTICLE_EFFECT_POOL_AMOUNT; i++) {
			var particleEffect = Instantiate(fireUpParticleEffect);
			particleEffect.SetActive(false);
			particleEffects.Add(particleEffect);
		}
	}

	private void CoordinateEvaluation() {
		if (evaluationRoutine != null) StopCoroutine(evaluationRoutine);
		evaluationRoutine = EvaluationRoutine();
		StartCoroutine(evaluationRoutine);
	}

	public void PlayBoardSetupAnimation(HashSet<BoardChipStack> cursedStackPositions) {
		cursedContainer = cursedStackPositions;
		IEnumerator coroutine = BoardSetupAnimation();
		StartCoroutine(coroutine);
	}

	private IEnumerator BoardSetupAnimation() {
		yield return beforeCurseWait;
		foreach (var chipStack in cursedContainer) {
			chipStack.AnimateCurse(true);
			yield return inbetweenCurseWait;
			GridManager.Instance.RegisterChainedTile();
		}
		yield return null;
	}

	private IEnumerator EvaluationRoutine() {
		while (stackPositionsToEvaluate.Count > 0) {
			yield return matchResolver.StartEvaluation(stackPositionsToEvaluate);
		
			if (completedStacks.Count > 0) {
				yield return inbetweenAnimWait;

				for (int i = 0; i < completedStacks.Count; i++) {
					comboSystem.IncreaseClearCombo(completedStacks[i].gameObject.transform.position);
					if (i == completedStacks.Count - 1) yield return completedStacks[i].PlayClearAnimation();
					else StartCoroutine(completedStacks[i].PlayClearAnimation());
					yield return inbetweenAnimWait;
				}

				completedStacks.Clear();
			}
		}

		MatchingConcluded?.Invoke();
	}

	public IEnumerator ProcessAnimationQueue(Queue<AnimationLink> animLinks) {
		while (animLinks.Count > 0) {
			currentLink = animLinks.Dequeue();
			currentFrom = GridManager.Instance.RequestTileUnsafe(currentLink.from).ChipStack;
			currentTo = GridManager.Instance.RequestTileUnsafe(currentLink.to).ChipStack;

			yield return currentFrom.StartMatchAnimation(currentTo, Mathf.Clamp(maxMatchAnimationDuration / currentFrom.ChipAmount, 0f, maxMatchAnimationDuration / 6));
			currentFrom = currentTo;
			yield return inbetweenAnimWait;
		}
	}

	public void RecordTilePlacement(Vector3Int pos) {
		stackPositionsToEvaluate.AddLast(pos);
	}

	public void RecordStackCompletion(BoardChipStack stack) {
		completedStacks.Add(stack);
	}

	public void RecordStackSplit(BoardChipStack stack) {
		stackPositionsToEvaluate.AddFirst(stack.OwnerTile.GridPosition);
	}

	private void OnEnable() {
		DealerBehaviour.HandEmpty += CoordinateEvaluation;
	}

	private void OnDisable() {
		DealerBehaviour.HandEmpty -= CoordinateEvaluation;
	}
}
