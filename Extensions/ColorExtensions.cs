using UnityEngine;

namespace Elarion {
	public static class ColorExtensions {
		public static Color EaseTo(this Color start, Color end, float value, Ease easeType = Ease.Linear) {
			return new Color(start.r.EaseTo(end.r, value, easeType), start.g.EaseTo(end.g, value, easeType),
							 start.b.EaseTo(end.b, value, easeType), start.a.EaseTo(end.a, value, easeType));
		}
	}
}