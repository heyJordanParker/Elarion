using UnityEngine;

namespace Elarion {

	public class DontDestroyOnLoad : ExtendedBehaviour {

		public int originatingLevel = -1;

		protected override void Initialize() {
			base.Initialize();
			originatingLevel = Application.loadedLevel;
			DontDestroyOnLoad(gameObject);
		}

	}

}