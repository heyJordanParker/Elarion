using System;
using UnityEngine;

namespace Elarion {

	[Serializable]
	public class Random {

		//TODO log all seeds and add an option to use the logged instead of the provided values

		[SerializeField, HideInInspector] private UnityRandom _random;

		public Random(int seed) {
			_random = new UnityRandom(seed);
		}
		
		public Random() {
			_random = new UnityRandom();
		}

		public float Percentage() { return _random.Value(UnityRandom.Normalization.STDNormal, 0.5f); }
		public float Range(int min, int max) { return Percentage() * (max - min) + min; }
		public float Range(float min, float max) { return Percentage() * (max - min) + min; }
		public int RangeInt(int min, int max) { return Mathf.RoundToInt(Range(min, max)); }

	}

}