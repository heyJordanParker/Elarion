using UTime = UnityEngine.Time;

namespace Elarion {
	public class Time {
		 
		public static float deltaTime { get { return UTime.deltaTime; } }
		public static float smoothDeltaTime { get { return UTime.smoothDeltaTime; } }
		public static float timeSinceLevelLoad { get { return UTime.timeSinceLevelLoad; } }
		public static float time { get { return UTime.time; } }
		public static float timeScale { get { return UTime.timeScale; } }
		public static float realtimeSinceStartup { get { return UTime.realtimeSinceStartup; } }
		

	}
}