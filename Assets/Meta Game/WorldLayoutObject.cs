using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLayoutObject : MonoBehaviour {
	[SerializeField] private WorldDataContainer dataContainer;
	[SerializeField] private List<LevelClusterWorld> levelClusters;
	[SerializeField] private WorldExit exitObject;
	[SerializeField] private int minRequiredStars;

	public Sprite starSprite;
	public Color starColour;
	private int currentStarCount;

	public List<LevelDataContainer> LevelData { get { return dataContainer.levelDataContainers; } }
	public WorldDataContainer DataContainer { get { return dataContainer; } }
	public int CurrentStarCount { get { return currentStarCount; } }
	public int RequiredStars { get { return minRequiredStars; } }

	public void OnWorldLoad(int starCount, List<LevelClusterUI> uiClusters) {
		currentStarCount = starCount;

		for (int i = 0; i < levelClusters.Count; i++) {
			if (i < uiClusters.Count) {
				levelClusters[i].AssignCluster(uiClusters[i], dataContainer.worldIndex, i, starSprite);
			}
		}
		// calculate total stars, not gonna save that, only creates more points of failure
		currentStarCount = starCount;
		exitObject.OnWorldLoad(this, StarQuotaReached(), minRequiredStars);
	}

	public bool StarQuotaReached() {
		if (currentStarCount >= minRequiredStars) return true;
		else return false;
	}
}
