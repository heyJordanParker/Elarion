using UTime = UnityEngine.Time;

namespace Elarion.General {
	public static class ETime {
		public static float DeltaTime => UTime.deltaTime;
		public static float UnscaledDeltaTime => UTime.unscaledDeltaTime;
		public static float SmoothDeltaTime => UTime.smoothDeltaTime;
		public static float TimeSinceLevelLoad => UTime.timeSinceLevelLoad;
		public static float Time => UTime.time;
		public static float TimeScale => UTime.timeScale;
		public static float RealtimeSinceStartup => UTime.realtimeSinceStartup;
	}
}