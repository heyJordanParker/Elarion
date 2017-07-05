using UnityEngine;

namespace Elarion.Extensions {
	public static class ComponentExtensions {

		public static void Prepare(this Component component) { }

	    public static void SetActive(this Component component, bool value) {
	        component.gameObject.SetActive(value);
	    }

	}
}