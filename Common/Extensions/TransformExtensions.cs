using UnityEngine;

namespace Elarion.Common.Extensions {
	public static class TransformExtensions {

		public static void ResetPosition(this Transform transform) { transform.localPosition = Vector3.zero; }
		public static void ResetRotation(this Transform transform) { transform.localRotation = Quaternion.identity; }
		public static void ResetScale(this Transform transform) { transform.localScale = Vector3.one; }

		public static void Reset(this Transform transform) {
			ResetPosition(transform);
			ResetRotation(transform);
			ResetScale(transform);
		}

		public static bool IsChildOf(this Transform child, GameObject parent) {
			return child.IsChildOf(parent.transform);
		}

		public static bool IsParentOf(this Transform parent, GameObject child) {
			return IsParentOf(parent, child.transform);
		}
		
		public static bool IsParentOf(this Transform parent, Transform child) {
			return child.IsChildOf(parent);
		}
	}
}