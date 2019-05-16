using UnityEngine;

namespace Elarion.Common.Extensions {
	public static class ColorExtensions {
		public static Color EaseTo(this Color start, Color end, float value, Ease easeType = Ease.Linear) {
			return new Color(start.r.EaseTo(end.r, value, easeType), start.g.EaseTo(end.g, value, easeType),
							 start.b.EaseTo(end.b, value, easeType), start.a.EaseTo(end.a, value, easeType));
		}

		/// <summary>
		/// Utility function to set a color's alpha easily. Alpha value is between 0 (transparent) and 1 (opaque).
		/// </summary>
		public static Color SetAlpha(this Color color, float alpha) {
			return new Color(color.r, color.g, color.b, alpha);
		}
	}
}