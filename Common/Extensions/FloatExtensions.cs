using UnityEngine;

namespace Elarion.Common.Extensions {
	public static class FloatExtensions {
		 
		public static float EaseTo(this float start, float end, float value, Ease easeType = Ease.Linear) { return Easing.Ease(start, end, value, easeType); }

		public static float Abs(this float value) {
			return Mathf.Abs(value);
		}

		public static float Sign(this float value) {
			return Mathf.Sign(value);
		}

		public static bool Approximately(this float value, float otherValue) {
			return Mathf.Approximately(value, otherValue);
		}

		public static bool InRange(this float value, float min, float max) {
			return value <= max && value >= min;
		}

		public static bool InRange(this float value, Vector2 minMaxVector) {
			return value.InRange(minMaxVector.x, minMaxVector.y);
		}
	}
}