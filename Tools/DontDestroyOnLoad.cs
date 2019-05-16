using UnityEngine;

namespace Elarion.Tools {
	public sealed class DontDestroyOnLoad : MonoBehaviour {
		private void Awake() {
			DontDestroyOnLoad(gameObject);
		}
	}
}