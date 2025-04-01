using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class ClickTileBehaviour : MonoBehaviour {
	[SerializeField] private SpriteRenderer sr;
	[SerializeField] private SpriteRenderer subSr;
	public LevelBuilderBehaviour builder;

	private Color defaultColour;
	private Color defaultSelectColour;
	private int stateCounter = 0;
	private int subStateCounter = 0;
	private bool filled = false;


	public bool isLuminary = false;
	public bool Filled { get { return filled; } }
	public int SubState { get { return subStateCounter; } }

	private void Start() {
		defaultColour = Color.black;
		sr.color = defaultColour;
		defaultSelectColour = Color.yellow;
		subSr.color = defaultColour;
		if (isLuminary) {
			defaultColour = Color.grey;
			defaultSelectColour = Color.blue;
			sr.color = defaultColour;
		}
	}

	public void AssignLuminaryStatus() {
		isLuminary = true;
		defaultColour = Color.grey;
		defaultSelectColour = Color.blue;
	}

	public void ChangeTileState(bool wasPrimary) {
		if (wasPrimary) ChangeTilePrimaryState();
		else ChangeTileSubState();
	}

	public void ChangeTilePrimaryState() {
		if (filled) {
			filled = false;
			sr.color = defaultColour;
			subSr.color = Color.black;
			subStateCounter = 0;
		}
		else {
			filled = true;
			sr.color = defaultSelectColour;
			subSr.color = Color.yellow;
			subStateCounter = 0;
		}
		builder.ChangeCounter(filled);
	}

	public void ChangeTileSubState() {
		if (!filled) {
			subStateCounter = 0;
			subSr.color = Color.black;
			return;
		}
		subStateCounter++;
		if (subStateCounter >= Enum.GetValues(typeof(ClickTileSubState)).Length) {
			subStateCounter = 0;
		}

		switch (subStateCounter) {
			case 0:
				subSr.color = Color.yellow; // empty, do whatever you like
				break;
			case 1:
				subSr.color = Color.red; // spawn tiles here
				break;
			case 2:
				subSr.color = Color.blue; // entity spawn or exit
				break;
			case 3:
				subSr.color = Color.black; // don't spawn tiles here
				break;
			case 4:
				subSr.color = Color.green; // special tile
				break;
			default:
				break;
		}
	}
}

public enum ClickTileSubState { 
	Empty,
	TileSpawn,
	EntitySpawn,
	BlockTileSpawn,
	SpecialTileSpawn // treat as normal tile, future proofing
}

