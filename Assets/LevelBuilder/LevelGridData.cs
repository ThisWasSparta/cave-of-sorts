using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Grid Data")]
public class LevelGridData : ScriptableObject {
	public List<Vector3Int> tilePositions;
	public List<Vector3Int> tilesToFill;
	public List<Vector3Int> blockedTiles;
	public List<Vector3Int> entityPositions;
	public List<Vector3Int> specialTilePositions;
}
