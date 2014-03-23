using UnityEngine;

namespace Elarion {

	/// <summary>
	/// Initializer base class - its used for initialization of
	/// the main state machine and the corresponding managers and systems.
	/// </summary>
	public class Initializer : ExtendedBehaviour {

		protected override void Initialize() {

			Application.runInBackground = true;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

//			Shader.WarmupAllShaders();
		}

	}

}

