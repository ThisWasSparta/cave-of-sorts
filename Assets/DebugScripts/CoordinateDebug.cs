using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CoordinateDebug : MonoBehaviour
{
	[SerializeField] private int minDrawX;
	[SerializeField] private int maxDrawX;
	[SerializeField] private int minDrawZ;
	[SerializeField] private int maxDrawZ;

	private Grid grid;

	private void Awake() {
		grid = GetComponent<Grid>();
	}

	/*private void OnGUI() { // remember, debug stuff cannot be in build
		for (int i = minDrawX; i < maxDrawX; i++) {
			for (int j = minDrawZ; j < maxDrawZ; j++) {
				Handles.Label(grid.CellToWorld(new Vector3Int(i, j, 0)), $"{i}, {j}");
			}
		}
	}//*/
}
