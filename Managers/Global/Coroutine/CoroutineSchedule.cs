using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion.CoreManagers {

	/// <summary>
	/// Manager class.
	/// The Coroutine Schedule Manager manages the creation of coroutines
	/// </summary>
	public class CoroutineSchedule : MonoBehaviour {

		public GameObject GameObject {
			get { if(_gameObject == null) _gameObject = base.gameObject; return _gameObject; }
		}

// ReSharper disable InconsistentNaming
		public new GameObject gameObject {
			get { if(_gameObject == null) _gameObject = base.gameObject; return _gameObject; }
		}
// ReSharper restore InconsistentNaming


		private List<IEnumerator> _newCoroutines;
		private GameObject _gameObject;

		private List<IEnumerator> NewCoroutines {
			get {
				return _newCoroutines ?? (_newCoroutines = new List<IEnumerator>());
			}
		}

		void Update() {
			if(NewCoroutines.Count <= 0) return;

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