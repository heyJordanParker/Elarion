using UnityEngine;

namespace Elarion.Extensions {
	public static class VectorExtensions {

		public static Vector2 EaseTo(this Vector2 start, Vector2 end, float value, Ease easeType = Ease.Linear) {
			return new Vector2(start.x.EaseTo(end.x, value, easeType), start.y.EaseTo(end.y, value, easeType));
		}

		public static Vector3 EaseTo(this Vector3 start, Vector3 end, float value, Ease easeType = Ease.Linear) {
			return new Vector3(start.x.EaseTo(end.x, value, easeType), start.y.EaseTo(end.y, value, easeType), 
							   start.z.EaseTo(end.z, value, easeType));
		}

		public static float DistanceXZ(this Vector3 x, Vector3 other) { return Vector2.Distance(new Vector2(x.x, x.z), new Vector2(other.x, other.z)); }

		public static Vector4 EaseTo(this Vector4 start, Vector4 end, float value, Ease easeType = Ease.Linear) {
			return new Vector4(start.x.EaseTo(end.x, value, easeType), start.y.EaseTo(end.y, value, easeType),
							   start.z.EaseTo(end.z, value, easeType), start.w.EaseTo(end.w, value, easeType));
		}

	}
}