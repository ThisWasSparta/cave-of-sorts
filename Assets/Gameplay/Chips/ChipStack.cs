using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ChipStack : MonoBehaviour {
	[SerializeField] protected Stack<Chip> chips;
	[SerializeField] private float inbetweenTileHeight = 0.01f;

	protected List<Chip> tempChipList;
	protected bool onGrid;

	public Stack<Chip> Chips { get { return chips; } }
	public int ChipAmount { get { return chips.Count; } }
	public bool IsEmpty { get { return chips.Count == 0; } }
	public bool OnGrid { get { return onGrid; } }
	public float InbetweenTileHeight { get { return inbetweenTileHeight; } }


	protected virtual void Awake() {
		chips = new Stack<Chip>();
	}

	public virtual void AddTilesRandom(List<ChipType> typesAvailable, int amount, int differentColours) {
		if (amount == 0) return;
		if (differentColours <= 0) differentColours = 1;
		if (differentColours > amount) 
			differentColours = amount;

		int chipsPerDivision = amount / differentColours;
		if (chipsPerDivision == 0) return;
		int remainder = amount % differentColours;
		int numberOfDivisions = (amount - remainder) / chipsPerDivision;
		int remainderReceiver = Random.Range(0, numberOfDivisions + 1);

		ChipType previousType = ChipType.Invalid;
		int chipCreated = 0;

		for (int i = 0; i < numberOfDivisions; i++) {
			ChipType currentType = GetRandomTypeWithExclusion(typesAvailable, previousType);
			for (int j = 0; j < chipsPerDivision; j++) {
				if (i == remainderReceiver) {
					for (int k = 0; k < remainder; k++) {
						CreateChip(currentType);
						chipCreated++;
					}
				}
				CreateChip(currentType);
				chipCreated++;
			}
			previousType = currentType;
		}

		UpdateTilePositions();
	}

	public void AddSpecificTiles(ChipType type, int amount) {
		for (int i = 0; i < amount; i++) {
			CreateChip(type);
		}
		UpdateTilePositions();
	}

	private void CreateChip(ChipType currentType) {
		var chip = ChipFactory.Instance.RetrieveTileObject(currentType);
		var chipScript = chip.GetComponent<Chip>();
		chips.Push(chipScript);
		chipScript.AssignOwner(this);
	}

	private ChipType GetRandomTypeWithExclusion(List<ChipType> typesAvailable, ChipType typeToExclude) {
		int whileRoguePrevention = 0;
		while (true) {
			whileRoguePrevention++;
			if (whileRoguePrevention > 1000) break;
			var randomType = typesAvailable[Random.Range(0, typesAvailable.Count)];
			if (randomType != typeToExclude) {
				return randomType;
			}
		}
		return typesAvailable[0];
	}

	protected virtual void UpdateTilePositions() {
		tempChipList = chips.ToList();
		tempChipList.Reverse();
		for (int i = 0; i < chips.Count; i++) {
			tempChipList[i].transform.parent = this.transform;
			tempChipList[i].AssignOwner(this);
			tempChipList[i].UpdatePosition(i);
		}

		tempChipList.Clear();
	}

	public bool TypeOnTop(out ChipType type) {
		type = ChipType.Invalid;
		if (chips.Count > 0) {
			type = chips.Peek().Type;
			return true;
		}
		else return false;
	}

	public void ClearChips() {
		chips.Clear();
	}
}
