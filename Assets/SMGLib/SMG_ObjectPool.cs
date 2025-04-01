using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG_ObjectPool // custom implementation of object pool cuz the new unity one scares me :((((
{
	private List<GameObject> pooledObjects;
	private Transform parent = default;
	private GameObject objectToPool = default;

	private int amountToPool;

	public SMG_ObjectPool(GameObject objectToPool, Transform parent, int amountToPool) {
		this.objectToPool = objectToPool;
		this.amountToPool = amountToPool;
		pooledObjects = new List<GameObject>();
		GameObject tmp;

		for (int i = 0; i < amountToPool; i++) {
			if (parent) tmp = GameObject.Instantiate(objectToPool, parent);
			else tmp = GameObject.Instantiate(objectToPool);
			tmp.SetActive(false);
			pooledObjects.Add(tmp);
		}
	}

	public GameObject GetPooledObject() {
		for (int i = 0; i < pooledObjects.Count; i++) {
			if (!pooledObjects[i].activeInHierarchy) {
				pooledObjects[i].SetActive(true);
				return pooledObjects[i];
			}
		}
		GrowPool();
		return GetPooledObject();
	}

	private void GrowPool() { // double it if not enough
		amountToPool *= 2;
		for (int i = pooledObjects.Count; i < amountToPool; i++) {
			GameObject tmp = GameObject.Instantiate(objectToPool);
			tmp.SetActive(false);
			pooledObjects.Add(tmp);
			if (parent) tmp.transform.SetParent(parent);
		}
	}
}
