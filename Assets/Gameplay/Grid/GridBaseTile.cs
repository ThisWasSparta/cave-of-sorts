using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridBaseTile : MonoBehaviour {
	[SerializeField] private SpriteRenderer sr;
	[SerializeField] private bool blockedTile = false;

	private Vector3Int gridPosition;

	private BoardChipStack chipStack;

	public BoardChipStack ChipStack { get { return chipStack; } }
	public Vector3Int GridPosition { get { return gridPosition; } }
	public bool IsBlocked { get { return blockedTile; } }

	private void Awake() {
		sr = GetComponentInChildren<SpriteRenderer>();
		chipStack = GetComponent<BoardChipStack>();
		chipStack.AssignOwner(this);
		if (blockedTile) sr.color = Color.black;
		else sr.color = Color.white;
	}

	public void SetGridPosition(Vector3Int position) {
		gridPosition = position;
	}
}
