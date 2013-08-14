using UnityEngine;

namespace Elarion {
	public static class GameObjectExtensions {

		public static bool HasComponent<T>(this GameObject go) where T : Component { return go.GetComponent<T>() != null; }
	
		public static T Component<T>(this GameObject go) where T : Component {
			var component = go.GetComponent<T>();
			if(component == null)
				component = go.AddComponent<T>();
			return component;
		}

		public static void SetLayer(this GameObject go, int layer) {
			go.layer = layer;
			foreach(Transform child in go.transform) {
				SetLayer(child.gameObject, layer);
			}
		}

	}
}

