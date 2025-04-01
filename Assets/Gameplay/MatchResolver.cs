using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MatchResolver : MonoBehaviour {
	public delegate void ResolverEvent(bool matchesFound);
	public delegate void EvaluationEvent();
	public static event EvaluationEvent EvaluationStarted; 
	public static event ResolverEvent BoardEvaluated;

	private GridManager gridManager;
	private AnimationCoordinator animCoordinator;
	private HashSet<Vector3Int> visitedPositions;
	private WaitForSeconds initialWait;
	private Queue<AnimationLink> animationLinks;
	private IEnumerator evaluationCoroutine;

	private bool matching = false;

	public bool Matching { get { return matching; } }

	const int HIGH_VALUE = 999;
	const int HEXAGON_NEIGHBOURS = 6;

	private void Start() {
		gridManager = GridManager.Instance;
		animCoordinator = GetComponent<AnimationCoordinator>();
		visitedPositions = new HashSet<Vector3Int>();
		initialWait = new WaitForSeconds(0.2f);
	}

	public IEnumerator StartEvaluation(LinkedList<Vector3Int> stackPositionsToEvaluate) {
		EvaluationStarted?.Invoke();
		yield return initialWait;
		int matchesFound = 0;
		while (stackPositionsToEvaluate.Count > 0) {
			MatchData data;
			var stackPos = stackPositionsToEvaluate.First.Value;
			stackPositionsToEvaluate.RemoveFirst();
			if (!EvaluateAndCreateMatchData(stackPos, out data)) continue;
			matchesFound++;
			ResolveAdjacency(data);
			matching = true;
			yield return DetermineAnimationOrder(data);
			matching = false;
		}

		BoardEvaluated?.Invoke(matchesFound > 0);
		stackPositionsToEvaluate.Clear();
	}

	private bool EvaluateAndCreateMatchData(Vector3Int placedTilePos, out MatchData data) {
		data = new MatchData(ChipType.Invalid);

		GridBaseTile placedBaseTile;
		if (!gridManager.TryRequestTile(placedTilePos, out placedBaseTile)) return false;

		ChipType type;
		if (!placedBaseTile.ChipStack.TryGetTopTileType(out type)) return false;

		var tilePos = gridManager.WorldToGridPos(placedBaseTile.transform.position);

		data.tilePositions.Add(tilePos);
		data.adjacencyDictionary[tilePos] = new List<Vector3Int>();
		data.type = type;

		RecursivelyFindReachableTilesOfSameType(placedTilePos, data);
		if (data.tilePositions.Count <= 1) return false;
		return true;
	}

	private void RecursivelyFindReachableTilesOfSameType(Vector3Int position, MatchData data) {
		var neighbourPositions = position.y % 2 == 0 ? gridManager.hexagonDirections : gridManager.hexagonDirectionsOffset;

		for (int i = 0; i < neighbourPositions.Length; i++) {
			GridBaseTile neighbourGridTile;
			var neighbourPos = position + neighbourPositions[i];
			if (data.tilePositions.Contains(neighbourPos)) continue;
			if (gridManager.TryRequestTile(neighbourPos, out neighbourGridTile)) {
				if (!neighbourGridTile.ChipStack.IsEmpty) {
					if (neighbourGridTile.ChipStack.GetTopTileTypeUnsafe() == gridManager.RequestTileUnsafe(position).ChipStack.GetTopTileTypeUnsafe()) {
						data.tilePositions.Add(neighbourPos);
						RecursivelyFindReachableTilesOfSameType(neighbourPos, data);
					}
				}
			}
		}
	}

	private void ResolveAdjacency(MatchData data) {
		for (int i = 0; i < data.tilePositions.Count; i++) {
			for (int j = 0; j < data.tilePositions.Count; j++) {
				if (i == j) continue;
				for (int k = 0; k < HEXAGON_NEIGHBOURS; k++) {
					var currentPos = data.tilePositions[i];
					var neighbourPositions = currentPos.y % 2 == 0 ? gridManager.hexagonDirections : gridManager.hexagonDirectionsOffset;
					var neighbourPos = currentPos + neighbourPositions[k];
					var adjacencyList = data.adjacencyDictionary;
					if (!data.tilePositions.Contains(neighbourPos)) continue;
					if (neighbourPos == data.tilePositions[j]) {
						if (!adjacencyList[currentPos].Contains(neighbourPos)) {
							adjacencyList[currentPos].Add(neighbourPos);
						}
						if (!adjacencyList.ContainsKey(neighbourPos)) adjacencyList[neighbourPos] = new List<Vector3Int>();
						if (!adjacencyList[neighbourPos].Contains(currentPos)) adjacencyList[neighbourPos].Add(currentPos);
					}
				}
			}
		}
	}

	private IEnumerator DetermineAnimationOrder(MatchData data) {
		animationLinks = new Queue<AnimationLink>();
		int numberOfTilesToVisit = data.adjacencyDictionary.Keys.Count;

		if (numberOfTilesToVisit <= 3) {
			SimplePathTrace(data);
		}
		else {
			LeafCollapsePathTrace(data);
		}
		yield return animCoordinator.ProcessAnimationQueue(animationLinks); // extract out to start evaluation function
		visitedPositions.Clear();
		yield return null;
	}

	private void SimplePathTrace(MatchData data) {
		int visitedPlusCurrent = visitedPositions.Count + data.tilePositions.Count;
		var currentPos = DetermineStartTile(data);
		visitedPositions.Add(currentPos);
		while (!(visitedPositions.Count >= visitedPlusCurrent)) {
			var currentPosAdjacency = data.adjacencyDictionary[currentPos];
			Vector3Int nextPos = new Vector3Int(HIGH_VALUE, HIGH_VALUE);
			if (currentPosAdjacency.Count == 1) {
				nextPos = currentPosAdjacency[0];
				animationLinks.Enqueue(new AnimationLink(currentPos, nextPos));
				visitedPositions.Add(nextPos);
				currentPos = nextPos;
			}
			else {
				int leastAdjacents = HIGH_VALUE;
				foreach (var adjacent in currentPosAdjacency) {
					int currentAdjacents = data.adjacencyDictionary[adjacent].Count;
					if (!visitedPositions.Contains(adjacent)) {
						if (currentAdjacents < leastAdjacents) {
							leastAdjacents = currentAdjacents;
							nextPos = adjacent;
						}
					}
				}

				animationLinks.Enqueue(new AnimationLink(currentPos, nextPos));
				visitedPositions.Add(nextPos);
				currentPos = nextPos;
			}
		}
	}

	private void LeafCollapsePathTrace(MatchData data) {
		List<Vector3Int> leaves = GatherLeaves(data.adjacencyDictionary);
		Vector3Int currentPos;
		int numberOfLeavesToCollapse = leaves.Count - 2;
		int leafTilesPruned = 0;

		for (int j = 0; j < numberOfLeavesToCollapse; j++) {
			var startingLeaf = DetermineNextLeaf(leaves);
			currentPos = startingLeaf;
			while (data.adjacencyDictionary[currentPos].Count < 3) {
				foreach (var nextPos in data.adjacencyDictionary[currentPos]) {
					if (!visitedPositions.Contains(nextPos)) {
						animationLinks.Enqueue(new AnimationLink(currentPos, nextPos));
						visitedPositions.Add(currentPos);
						currentPos = nextPos;
						leafTilesPruned++;
					}
				}
			}

			leaves.Remove(startingLeaf);
		}

		int visitedPlusCurrent = visitedPositions.Count + (data.tilePositions.Count - leafTilesPruned);
		currentPos = DetermineStartTile(data);
		visitedPositions.Add(currentPos);
		while (visitedPositions.Count < visitedPlusCurrent) {
			int leastAdjacents = HIGH_VALUE;

			Vector3Int destination = Vector3Int.zero;
			foreach (var potentialNextPos in data.adjacencyDictionary[currentPos]) {
				int currentNumberAdjacents = data.adjacencyDictionary[potentialNextPos].Count;
				if (!visitedPositions.Contains(potentialNextPos)) {
					if (currentNumberAdjacents < leastAdjacents) {
						leastAdjacents = currentNumberAdjacents;
						destination = potentialNextPos;
					}
				}
			}

			animationLinks.Enqueue(new AnimationLink(currentPos, destination));
			visitedPositions.Add(destination);
			currentPos = destination;
		}
	}

	private Vector3Int DetermineNextLeaf(List<Vector3Int> leaves) {
		int minX = HIGH_VALUE;
		int minY = HIGH_VALUE;
		foreach (var pos in leaves) {
			if (visitedPositions.Contains(pos)) continue;
			if (pos.y > minY) continue;
			if (pos.y == minY && pos.x > minX) continue;
			minX = pos.x;
			minY = pos.y;
		}

		return new Vector3Int(minX, minY);
	}

	private List<Vector3Int> GatherLeaves(Dictionary<Vector3Int, List<Vector3Int>> adjacencyList) {
		var leaves = new List<Vector3Int>();
		foreach (var position in adjacencyList.Keys) {
			if (adjacencyList[position].Count == 1) {
				leaves.Add(position);
			}
		}
		return leaves;
	}

	private Vector3Int DetermineStartTile(MatchData dataContainer) {
		int minX = HIGH_VALUE;
		int minY = HIGH_VALUE;
		int minLeafX = HIGH_VALUE;
		int minLeafY = HIGH_VALUE;
		bool leafFound = false;
		foreach (var pos in dataContainer.adjacencyDictionary.Keys) {
			if (visitedPositions.Contains(pos)) continue;
			if (dataContainer.adjacencyDictionary[pos].Count == 1) {
				if (pos.y > minLeafY) continue;
				if (pos.y == minLeafY && pos.x > minLeafX) continue;
				minLeafX = pos.x;
				minLeafY = pos.y;
				leafFound = true;
			}
			if (pos.y > minY) continue;
			if (pos.y == minY && pos.x > minX) continue;
			minX = pos.x;
			minY = pos.y;
		}

		if (leafFound) return new Vector3Int(minLeafX, minLeafY);
		else return new Vector3Int(minX, minY);
	}

	/*private void OnDrawGizmos() {
		if (stackPositionsToEvaluate.Count > 0) {
			foreach (var pos in stackPositionsToEvaluate) {
				var jemoeder = gridManager.grid.CellToWorld(pos) + Vector3.up;
				Gizmos.DrawSphere(jemoeder, 0.1f);
			}
		}
	}*/
}
