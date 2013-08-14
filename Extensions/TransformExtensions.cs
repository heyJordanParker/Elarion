using UnityEngine;

namespace Elarion {
	public static class TransformExtensions {

		public static void ResetPosition(this Transform transform) { transform.localPosition = Vector3.zero; }
		public static void ResetRotation(this Transform transform) { transform.localRotation = Quaternion.identity; }
		public static void ResetScale(this Transform transform) { transform.localScale = Vector3.one; }

		public static void Reset(this Transform transform) {
			ResetPosition(transform);
			ResetRotation(transform);
			ResetScale(transform);
		}

	}
}