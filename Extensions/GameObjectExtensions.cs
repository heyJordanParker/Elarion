using Elarion.Managers;
using UnityEngine;

namespace Elarion {
	public static class GameObjectExtensions {
		public static void SetLayer(this GameObject go, int layer) {
			go.layer = layer;
			foreach(Transform child in go.transform) {
				SetLayer(child.gameObject, layer);
			}
		}

		public static Pool Pool(this GameObject go, uint amount) {
			var poolingManager = Singleton.Get<PoolingManager>();
			return poolingManager.Pool(go, amount);
		}

	}
}

