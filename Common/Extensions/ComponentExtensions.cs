using UnityEngine;

namespace Elarion.Common.Extensions {
	public static class ComponentExtensions {
		
		public static bool HasComponent<T>(this Component monoBehaviour) where T : Component {
			if(!monoBehaviour) {
				return false;
			}

			T component;
			return monoBehaviour.gameObject.HasComponent(out component);
		}

		public static bool HasComponent<T>(this Component monoBehaviour, out T component) where T : Component {
			if(!monoBehaviour) {
				component = null;
				return false;
			}
            
			return monoBehaviour.gameObject.HasComponent(out component);
		}

	    public static void SetActive(this Component component, bool value) {
	        component.gameObject.SetActive(value);
	    }

	}
}