using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BoardChipStack : ChipStack {
	[SerializeField] private List<ChipType> chipsToSpawn;
	[SerializeField] private ChipStackInfo infoScript;
	[SerializeField] private StackChainEffect chainEffect;

	[SerializeField] private float disappearAnimationDelay = 0.3f;
	[SerializeField] private float matchPacingDelayDuration = 0.1f;
	[SerializeField] private float clearPacingDelayDuration = 0.3f;

	private GridBaseTile gridTileOwner;
	private WaitForSeconds pacingDelay;
	private Coroutine finalMatchRoutine;
	private Coroutine finalClearRoutine;
	private bool chained;

	private const int FULL_STACK_AMOUNT = 8;

	public GridBaseTile OwnerTile { get { return gridTileOwner; } }
	public ChipStackInfo StackInfo { get { return infoScript; } }

	protected override void Awake() {
		base.Awake();
		pacingDelay = new WaitForSeconds(matchPacingDelayDuration);
	}

	private void Start() {
		disappearAnimationDelay = 0.5f;
		InstantiateTiles();
		UpdateCounter();
	}

	private void InstantiateTiles() {
		foreach (var chip in chipsToSpawn) {	
			var chipObj = ChipFactory.Instance.RetrieveTileObject(chip);
			var chipScript = chipObj.GetComponent<Chip>();
			chips.Push(chipScript);
			chipScript.AssignOwner(this);
			chipScript.UpdatePosition();
		}
	}

	public IEnumerator StartMatchAnimation(BoardChipStack destination, float duration) {
		if (chained) {
			yield return chainEffect.StartOutAnimation();
			GridManager.Instance.RegisterChainRelease();
			chained = false;
		} 
		if (destination.chained) {
			StartCoroutine(destination.chainEffect.StartOutAnimation());
			GridManager.Instance.RegisterChainRelease();
			destination.chained = false;
		} 

		WaitForSeconds inbetweenAnimDelay = new WaitForSeconds(duration);
		int chipCount = CountTopChipsOfSameType();
		if (chipCount == 0) yield break;
		if (chips.Count - chipCount > 0) AnimationCoordinator.Instance.RecordStackSplit(this);

		for (int i = 0; i < chipCount; i++) {
			var chip = chips.Pop();
			if (i == 1) finalMatchRoutine = StartCoroutine(chip.AnimateTo(destination));
			else StartCoroutine(chip.AnimateTo(destination));

			UpdateCounter();
			yield return inbetweenAnimDelay;
		}

		UpdateCounter();
		yield return finalMatchRoutine;

	}

	public IEnumerator PlayClearAnimation() {
		int chipCount = CountConsecutiveTypeFromTop();
		ChipType type = chips.Peek().Type;
		if (chips.Count - chipCount > 0) AnimationCoordinator.Instance.RecordStackSplit(this);
		for (int i = 0; i < chipCount; i++) {
			var chip = chips.Pop();
			if (i == chipCount - 1) finalClearRoutine = StartCoroutine(chip.AnimateAway());
			else StartCoroutine(chip.AnimateAway());
			UpdateCounter();
		}

		yield return finalClearRoutine;
		GridManager.Instance.RegisterMatch(type, chipCount);
	}

	private int CountTopChipsOfSameType() {
		int count = 1;
		var type = chips.Peek().Type;
		var chipList = chips.ToList();
		for (int i = 1; i < chipList.Count; i++) {
			if (chipList[i].Type == type) {
				count++;
			}
			else {
				return count;
			}
		}
		return count;
	}

	private int CountConsecutiveTypeFromTop() {
		int amount = 0;
		if (chips.Count == 0) return amount;

		var type = chips.Peek().Type;
		foreach (var chip in chips) {
			if (chip.Type == type) amount++;
			else break;
		}

		return amount;
	}

	public void AddChipsToStack(ChipStack chipsToAdd) {
		chips = new Stack<Chip>(chips.Union(chipsToAdd.Chips).Reverse());
		chipsToAdd.ClearChips();
		UpdateTilePositions();
		UpdateCounter();
	}

	public bool TryGetTopTileType(out ChipType type) {
		type = ChipType.Invalid;
		if (!IsEmpty) {
			type = chips.Peek().Type;
			return true;
		}
		else return false;
	}

	public ChipType GetTopTileTypeUnsafe() {
		if (!IsEmpty) return chips.Peek().Type;
		else return ChipType.Invalid;
	}

	public void AddChip(Chip chipToAdd) {
		chips.Push(chipToAdd);
		UpdateCounter();
	}

	public void AssignOwner(GridBaseTile tile) {
		gridTileOwner = tile;
		onGrid = true;
	}

	private void CheckIfStackIsFull(bool _) {
		var chipCount = CountConsecutiveTypeFromTop();
		if (chipCount >= FULL_STACK_AMOUNT) {
			AnimationCoordinator.Instance.RecordStackCompletion(this);
		}
	}

	public void ApplyCurse() {
		chained = true;
	}

	public void AnimateCurse(bool animateIn) {
		if (animateIn) {
			chainEffect.AnimateIn();
		} 
		else {
			chainEffect.AnimateOut();
		}
	}

	public void UpdateCounter() {
		int count = CountConsecutiveTypeFromTop();
		if (count == FULL_STACK_AMOUNT) infoScript.PlayStackCompletionAnimation();
		infoScript.UpdateCounter(count, chips.Count - count, InbetweenTileHeight);
	}

	private void OnEnable() {
		MatchResolver.BoardEvaluated += CheckIfStackIsFull;
	}

	private void OnDisable() {
		MatchResolver.BoardEvaluated -= CheckIfStackIsFull;

	}
}
