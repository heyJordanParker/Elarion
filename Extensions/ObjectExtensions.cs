using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion {
	public static class ObjectExtensions {

		//Instantiates which accept an Object parameter and automatically get a component game object or w/e possible
		//Instantiates which define a spawn layer || or delete that functionality entirely, because the original game object layer is restored

		public static T Instantiate<T>(this T spawn) where T : Component {
			return Managers.Pooling.Instantiate(spawn, Vector3.zero, Quaternion.identity) as T;
		}
		public static GameObject Instantiate(this GameObject go) {
			return Managers.Pooling.Instantiate(go, Vector3.zero, Quaternion.identity);
		}
		public static T Instantiate<T>(this T component, Transform t) where T : Component {
			return Managers.Pooling.Instantiate(component, t.position, t.rotation) as T;
		}
		public static GameObject Instantiate(this GameObject go, Transform t) {
			return Managers.Pooling.Instantiate(go, t.position, t.rotation);
		}
		public static T Instantiate<T>(this T componenet, Vector3 position, Quaternion rotation) where T : Component {
			return Managers.Pooling.Instantiate(componenet, position, rotation) as T;
		}
		public static T Instantiate<T>(this T componenet, Vector3 position, Quaternion rotation, int layer) where T : Component {
			return Managers.Pooling.Instantiate(componenet, position, rotation, layer) as T;
		}
		public static GameObject Instantiate(this GameObject go, Vector3 position, Quaternion rotation) {
			Debug.Log("used");
			return Managers.Pooling.Instantiate(go);
		}

		//layer, position, rotation
		public static void Spawn(this GameObject gameObject, int layer) {
			Managers.Pooling.Spawn(gameObject, layer);
		}

		public static void Spawn(this GameObject gameObject) {
			Managers.Pooling.Spawn(gameObject);
		}

		public static void Destroy(this GameObject gameObject) {
			Managers.Pooling.Destroy(gameObject);
		}

		public static void EmptyPool(this GameObject gameObject) {
			Managers.Pooling.EmptyPool(gameObject);
		}

		public static void EmptyPool(this Component component) {
			Managers.Pooling.EmptyPool(component.gameObject);
		}

		public static void Pool(GameObject gameObject, uint amount) {
			Managers.Pooling.Pool(gameObject, amount);
		}

		public static void Pool(this Component component, uint amount) {
			Managers.Pooling.Pool(component.gameObject, amount);
		}

	}
}
