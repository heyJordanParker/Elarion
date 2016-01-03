using UnityEngine;

namespace Elarion {

	public class DontDestroyOnLoad : MonoBehaviour {

		public int originatingLevel = -1;

		protected void Awake() {
			originatingLevel = Application.loadedLevel;
			DontDestroyOnLoad(gameObject);
		}

	}

}