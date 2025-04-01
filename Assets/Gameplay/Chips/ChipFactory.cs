using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ChipFactory : MonoBehaviour {
	public static ChipFactory Instance;

	[SerializeField] private List<GameObject> hexagonTilePrefabs;
	private SMG_ObjectPool[] poolManifest;

	private void Awake() {
		if (!Instance) {
			Instance = this;
		} else {
			if (Instance && Instance != this) {
				Destroy(this.gameObject);
			}
		}

		poolManifest = new SMG_ObjectPool[hexagonTilePrefabs.Count];
	}

	public GameObject RetrieveTileObject(ChipType type) {
		if (poolManifest[(int)type] == null) {
			CreatePool(type);
		}
		return poolManifest[(int)type].GetPooledObject();
	}

	public void RequestPoolCreation(ChipType type) {
		if (poolManifest[(int)type] == null) {
			CreatePool(type);
		} else {
			Debug.LogWarning("Object pool of requested type already exists!");
		}
	}

	private void CreatePool(ChipType type) {
		poolManifest[(int)type] = new SMG_ObjectPool(hexagonTilePrefabs[(int)type], null, 32);
	}
}
