using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Data Container")]
public class LevelDataContainer : ScriptableObject {
	public LevelGridData gridData;
	public LevelContentData contentData;
	public int dealerLevel;
}
