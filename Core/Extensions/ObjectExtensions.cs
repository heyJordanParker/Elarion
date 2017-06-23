using System;
using Elarion.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion.Extensions {
	public static class ObjectExtensions {
		public static void Destroy(this Object obj, float time = 0.0f) {
#if UNITY_EDITOR
			if(Application.isPlaying) {
				Object.Destroy(obj, time);
			} else {
				if(time > 0.0f)
					Debug.LogException(new Exception("Cannot use delayed destroy in edit mode. Using ImmediateDestroy instead."));
				Object.DestroyImmediate(obj);
			}
#else
			Object.Destroy(obj, time);
#endif
		}

		public static T Instantiate<T>(this T component, Transform transform, int layer = -1) where T : Component {
			if(component == null) throw new ArgumentNullException("component");
			if(transform == null) throw new ArgumentNullException("transform");
			return InstantiateOrSpawnFromPool(component.gameObject, transform.position, transform.rotation, layer).GetComponent<T>();
		}

		private static GameObject InstantiateOrSpawnFromPool(GameObject gameObject, Vector3 position, Quaternion rotation, int layer = -1) {
			//instantiate or spawn from pool
			return Singleton.Get<PoolingManager>().Instantiate(gameObject, position, rotation, layer);
		}

		public static T Instantiate<T>(this T component, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(), int layer = - 1) where T : Component {
			if(component == null) throw new ArgumentNullException("component");
			return InstantiateOrSpawnFromPool(component.gameObject, position, rotation, layer).GetComponent<T>();
		}

		public static GameObject Instantiate(this GameObject gameObject, Vector3 position, Quaternion rotation, int layer = -1) {
			if(gameObject == null) throw new ArgumentNullException("gameObject");
			return InstantiateOrSpawnFromPool(gameObject, position, rotation, layer);
		}
	}
}
