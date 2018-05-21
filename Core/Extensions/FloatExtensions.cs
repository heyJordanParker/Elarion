using UnityEngine;

namespace Elarion.Extensions {
	public static class FloatExtensions {
		 
		public static float EaseTo(this float start, float end, float value, Ease easeType = Ease.Linear) { return Easing.Ease(start, end, value, easeType); }

		public static float Abs(this float value) {
			return Mathf.Abs(value);
		}

		public static float Sign(this float value) {
			return Mathf.Sign(value);
		}
	}
}