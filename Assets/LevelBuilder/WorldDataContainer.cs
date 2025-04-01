using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World Data Container")]
public class WorldDataContainer : ScriptableObject {
	public int worldIndex;
	public List<LevelDataContainer> levelDataContainers;
	public Sprite starSprite;
	public List<LevelGridData> gridDataList;
	public List<LevelContentData> contentDataSets;
}
