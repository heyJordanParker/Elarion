using UnityEngine;

namespace Elarion {

	public class DontDestroyOnLoad : MonoBehaviour {

		public int originatingLevel = -1;

		protected void Awake() {
			originatingLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
			DontDestroyOnLoad(gameObject);
		}

	}

}