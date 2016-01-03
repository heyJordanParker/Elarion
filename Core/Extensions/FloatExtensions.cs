namespace Elarion {
	public static class FloatExtensions {
		 
		public static float EaseTo(this float start, float end, float value, Ease easeType = Ease.Linear) { return Easing.Ease(start, end, value, easeType); }

	}
}