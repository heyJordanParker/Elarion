using UnityEngine;

namespace Elarion.Utility {
	public sealed class DontDestroyOnLoad : MonoBehaviour {
		private void Awake() {
			DontDestroyOnLoad(gameObject);
		}
	}
}