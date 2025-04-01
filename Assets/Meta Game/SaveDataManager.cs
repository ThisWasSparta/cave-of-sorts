using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveDataManager {
	private Dictionary<string, bool[]> levelDictionary;
	private readonly int LEVELS_PER_WORLD = 20;

	public void SaveGameData() {
		if (!Directory.Exists("SaveFile"))
			Directory.CreateDirectory("SaveFile");

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream saveFile = File.Create("SaveFile/save.bin");

		formatter.Serialize(saveFile, levelDictionary);
		saveFile.Close();
	}

	public void LoadOrCreateData() {
		if (!File.Exists("SaveFile/save.bin")) {
			levelDictionary = new Dictionary<string, bool[]>();
			return;
		}

		BinaryFormatter formatter = new BinaryFormatter(); // apparently binary isn't particularly safe, but if you can somehow hack the pentagon by changing this save file in a text editor, i'll buy you a steak dinner
		using FileStream saveFile = File.Open("SaveFile/save.bin", FileMode.Open);
		levelDictionary = (Dictionary<string, bool[]>)formatter.Deserialize(saveFile);
		saveFile.Close();
	}

	public void RegisterLevelScore(string levelKey, bool[] stars) {
		if (levelDictionary.ContainsKey(levelKey)) {
			levelDictionary[levelKey] = stars;
		}
		else {
			levelDictionary.Add(levelKey, stars);
		}
		SaveGameData();
	}

	public bool TryGetStars(string levelKey, out bool[] stars) {
		return levelDictionary.TryGetValue(levelKey, out stars);
	}

	public int CountStarsInWorld(int worldIndex) {
		int count = 0;

		for (int i = 0; i < LEVELS_PER_WORLD; i++) {
			bool[] starData;
			if (TryGetStars($"{worldIndex}{i}", out starData)) {
				foreach (var star in starData) {
					if (star == true) count++;
				}
			}
		}
		return count;
	}
}
