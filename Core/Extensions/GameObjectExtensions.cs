using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Extensions {
	public static class GameObjectExtensions {

		public static bool HasComponent<T>(this GameObject gameObject) where T : Component {
			T component;
			if(!gameObject) {
				return false;
			}
			
			return gameObject.HasComponent<T>(out component);
		}
		
		public static bool HasComponent<T>(this GameObject gameObject, out T component) where T : Component {
			if(!gameObject) {
				component = null;
				return false;
			}
			
			component = gameObject.GetComponent<T>();

			if(!component) {
				return false;
			}

			if(component is MonoBehaviour && !(component as MonoBehaviour).enabled) {
				return false;
			}
			
			return true;
		}

		public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
			var component = go.GetComponent<T>();
			if(component == null)
				component = go.AddComponent<T>();
			return component;
		}

		public static void SetLayer(this GameObject go, int layer, bool recursive = true) {
			go.layer = layer;
			if(!recursive) return;
			foreach(Transform child in go.transform) {
				SetLayer(child.gameObject, layer);
			}
		}

		public static IEnumerable<Selectable> GetSelectableChildren(this GameObject go) {
			return go.GetComponentsInChildren<Selectable>();
		}

		public static Selectable GetFirstSelectableChild(this GameObject go) {
			return go.GetSelectableChildren().FirstOrDefault();
		}
	}
}

