using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG_WeightedRandomDistribution { // implementation courtesy of Timothy Groote on stackoverflow
	private Vector2Int[] parameters;

	public SMG_WeightedRandomDistribution(Vector2Int[] parameters) {
		this.parameters = parameters;
	}

	public int GenerateValue() {
		int value = (int)(UnityEngine.Random.value * CountSum());
		int count = 0;

		foreach (var parameter in parameters) {
			value -= parameter.x;

			if (!(value <= 0)) {
				count++;
				continue;
			}
			return parameter.y;
		}
		return 0;
	}

	private int CountSum() {
		int count = 0;
		foreach (var parameter in parameters) {
			count += parameter.x;
		}

		return count;
	}
}