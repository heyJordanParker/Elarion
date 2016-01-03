using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion.Managers {

	/// <summary>
	/// Manager class.
	/// The Coroutine Schedule Manager manages the creation of coroutines
	/// </summary>
	public class CoroutineSchedule : Singleton {
		private List<IEnumerator> _newCoroutines;

		private List<IEnumerator> NewCoroutines {
			get {
				return _newCoroutines ?? (_newCoroutines = new List<IEnumerator>());
			}
		}

		void Update() {
			if(NewCoroutines.Count <= 0)
				return;

			lock(NewCoroutines) {
				while(NewCoroutines.Count > 0) {
					StartCoroutine(NewCoroutines[0]);
					NewCoroutines.RemoveAt(0);
				}
			}
		}

		internal CoroutineInternal CreateCoroutine(IEnumerator coroutine, GameObject gameObject) {
			return new CoroutineInternal(coroutine, gameObject);
		}

	}

}