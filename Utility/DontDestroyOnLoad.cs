using UnityEngine;

namespace Elarion {

	public class DontDestroyOnLoad : ExtendedBehaviour {

		public int originatingLevel = -1;

		protected void Awake() {
			originatingLevel = Application.loadedLevel;
			DontDestroyOnLoad(gameObject);
		}

	}

}