using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MatchData {
	public ChipType type;
	public List<Vector3Int> tilePositions;
	public Dictionary<Vector3Int, List<Vector3Int>> adjacencyDictionary;

	public MatchData(ChipType type) {
		this.type = type;
		tilePositions = new List<Vector3Int>();
		adjacencyDictionary = new Dictionary<Vector3Int, List<Vector3Int>>();
	}
}

public struct AnimationLink {
	public Vector3Int from;
	public Vector3Int to;

	public AnimationLink(Vector3Int from, Vector3Int to) {
		this.from = from;
		this.to = to;
	}
}
