using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Data")]
public class LevelContentData : ScriptableObject
{
	public List<ChipType> tilesPresentInLevel;
	public List<TrackedObjective> levelObjectives;
}
