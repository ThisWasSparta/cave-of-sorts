using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DealerBehaviour : MonoBehaviour {
	public delegate void DealerEvent();
	public static event DealerEvent HandEmpty;

	[SerializeField] private GameObject dealerStackObject;
	[SerializeField] private GameObject selectionShine;
	[SerializeField] private List<GameObject> currentlyHeldStacks;
	[SerializeField] private List<RectTransform> stacktransformBounds;
	[SerializeField] private HitButton hitButton;
	[SerializeField] private LayerMask geometryLayer;
	[SerializeField] private float depthValue = 14.3f;
	[SerializeField] private int maxStacksDealt;
	[SerializeField] private int startMaxStacksDealt;

	[SerializeField] private Vector3 leftStackPos;
	[SerializeField] private Vector3 midStackPos;
	[SerializeField] private Vector3 rightStackPos;

	private SMG_ObjectPool chipStackPool;
	private SMG_WeightedRandomDistribution chipAmountGenerator;
	private SMG_WeightedRandomDistribution colourAmountGenerator;
	private List<Vector2Int[]> colourPerStackParameters;
	private List<Vector2Int[]> chipsInStackParameters;
	private IEnumerator selectionShineRoutine;
	private int currentMaxAmountOfStacksDealt;
	private bool animatingShine;

	public List<GameObject> CurrentlyHeldStacks { get { return currentlyHeldStacks; } }
	public SMG_WeightedRandomDistribution ChipAmountGenerator { get { return chipAmountGenerator; } }
	public SMG_WeightedRandomDistribution ColourAmountGenerator { get { return colourAmountGenerator; } }

	private void Start() {
		chipStackPool = new SMG_ObjectPool(dealerStackObject, this.transform, 10);
		currentMaxAmountOfStacksDealt = startMaxStacksDealt;
		currentlyHeldStacks = new List<GameObject>();
	}

	public void InitializeDealer(int level) {
		GenerateParameters();
		colourAmountGenerator = new SMG_WeightedRandomDistribution(colourPerStackParameters[level]);
		chipAmountGenerator = new SMG_WeightedRandomDistribution(chipsInStackParameters[level]);
		GenerateChipStackBatch(true);
	}

	private void GenerateChipStackBatch(bool triggeredDuringSetup) {
		for (int i = 0; i < currentMaxAmountOfStacksDealt; i++) {
			if (!triggeredDuringSetup && currentlyHeldStacks.Count >= GridManager.Instance.EmptyGridTileCount()) hitButton.SetButtonActive(false);
			GenerateChipStack(triggeredDuringSetup);
		}
		UpdateHeldStackPositions();
	}

	public void DealSpecificChipStack(ChipType type, int amount, out Vector3 stackPos) {
		var stack = chipStackPool.GetPooledObject();
		stack.SetActive(true);
		stack.GetComponent<ChipStack>().AddSpecificTiles(type, amount);
		currentlyHeldStacks.Add(stack);
		UpdateHeldStackPositions();
		stackPos = stack.transform.position;
	}

	public void DealSpecificChipStack(Tuple<ChipType, int>[] chips, out Vector3 stackPos) { // everything about this fucking sucks, fix it
		var stackObj = chipStackPool.GetPooledObject();
		stackObj.SetActive(true);
		var stack = stackObj.GetComponent<ChipStack>();
		stack.ClearChips();
		currentlyHeldStacks.Add(stackObj);
		foreach (var colourStack in chips) {
			stack.AddSpecificTiles(colourStack.Item1, colourStack.Item2);
		}

		UpdateHeldStackPositions();
		stackPos = stack.transform.position;
	}

	private void GenerateChipStack(bool triggeredDuringSetup) {
		var dealerStack = chipStackPool.GetPooledObject();
		dealerStack.SetActive(true);
		dealerStack.GetComponent<ChipStack>().AddTilesRandom(GridManager.Instance.GetTileTypes(), chipAmountGenerator.GenerateValue(), colourAmountGenerator.GenerateValue());
		currentlyHeldStacks.Add(dealerStack);
		if (triggeredDuringSetup) return; // little hackey, but when setting up the button gets erroneously disabled cuz the grid isn't ready yet
		if (currentlyHeldStacks.Count == 0 || currentlyHeldStacks.Count >= maxStacksDealt || currentlyHeldStacks.Count >= GridManager.Instance.EmptyGridTileCount()) hitButton.SetButtonActive(false);
	}

	private void UpdateHeldStackPositions() {
		if (currentlyHeldStacks.Count == 1) {
			currentlyHeldStacks[0].transform.position = midStackPos;
			return;
		}

		if (currentlyHeldStacks.Count == 2) {
			currentlyHeldStacks[0].transform.position = leftStackPos;
			currentlyHeldStacks[1].transform.position = rightStackPos;
			return;
		}

		if (currentlyHeldStacks.Count == 3) {
			currentlyHeldStacks[0].transform.position = leftStackPos;
			currentlyHeldStacks[1].transform.position = midStackPos;
			currentlyHeldStacks[2].transform.position = rightStackPos;
			return;
		}
	}

	public void DealChipStack() { // used by button
		if (currentlyHeldStacks.Count >= maxStacksDealt) return;
		GenerateChipStack(false);
		UpdateHeldStackPositions();
	}

	public void RemoveStackFromHand(GameObject placedStack, bool tutorialMode) {
		if (!currentlyHeldStacks.Contains(placedStack)) {
			Debug.LogWarning("tried removing stack from dealer's hand that wasn't in there anyway, smth's going wrong here");
			return;
		}

		currentlyHeldStacks.Remove(placedStack);
		placedStack.SetActive(false);
		if (!tutorialMode) CheckIfHandIsEmpty(true);
	}

	public void EnableSelectionShine(GameObject selectedStack) {
		selectionShine.transform.position = selectedStack.transform.position;
		selectionShine.SetActive(true);
	}

	public void DisableSelectionShine() {
		selectionShine.SetActive(false);
	}

	private void CheckIfHandIsEmpty(bool generateBatchIfEmpty) {
		if (currentlyHeldStacks.Count > 0) return;

		HandEmpty?.Invoke();
		if (generateBatchIfEmpty) GenerateChipStackBatch(false);
		hitButton.SetButtonActive(true);
	}

	public void ForceHandEmptySignal() { // for use *ONLY* in tutorial context 
		HandEmpty?.Invoke();
	}

	public void SetHitButtonState(bool state) {
		hitButton.SetButtonActive(state);
	}

	private void GenerateParameters() { // yes this is really ugly, should probably do this in scriptableobject, for now this works
		colourPerStackParameters = new List<Vector2Int[]> {
			new Vector2Int[3] {
				new Vector2Int(30, 1),
				new Vector2Int(60, 2),
				new Vector2Int(10, 3)
			},
			new Vector2Int[3] {
				new Vector2Int(10, 1),
				new Vector2Int(50, 2),
				new Vector2Int(30, 3)
			},
			new Vector2Int[3] {
				new Vector2Int(30, 2),
				new Vector2Int(30, 3),
				new Vector2Int(5, 4)
			}
		};

		chipsInStackParameters = new List<Vector2Int[]> {
			new Vector2Int[5] {
				new Vector2Int(10, 3),
				new Vector2Int(30, 4),
				new Vector2Int(40, 5),
				new Vector2Int(30, 6),
				new Vector2Int(10, 7),
			},
			new Vector2Int[5] {
				new Vector2Int(10, 3),
				new Vector2Int(30, 4),
				new Vector2Int(40, 5),
				new Vector2Int(30, 6),
				new Vector2Int(10, 7),
			},
			new Vector2Int[4] {
				new Vector2Int(30, 4),
				new Vector2Int(40, 5),
				new Vector2Int(40, 6),
				new Vector2Int(10, 7),
			}
		};
	}
}