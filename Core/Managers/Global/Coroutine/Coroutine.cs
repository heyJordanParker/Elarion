using System;
using System.Collections;
using UnityEngine;

namespace Elarion {

	/// <summary>
	/// Coroutine class. Wrapper class for managed coroutines.
	/// Provides an easy interface for creation and management of coroutines.
	/// </summary>
	public class Coroutine : ExtendedBehaviour {

		public event Action<bool> OnFinished;
		public new string name;

		private CoroutineInternal _coroutine;

		public bool Running { get { return _coroutine.Running; } }
		public bool Paused { get { return _coroutine.Paused; } set { _coroutine.Paused = value; } }

		public static Coroutine Create(GameObject gameObject, IEnumerator iEnumerator) { return Create(gameObject, iEnumerator, null); }

		public static Coroutine Create(GameObject gameObject, IEnumerator iEnumerator, string name) { return Create(gameObject, iEnumerator, name, true); }

		public static Coroutine Create(GameObject gameObject, IEnumerator iEnumerator, string name, bool start) {
			if(gameObject == null) {
				if(string.IsNullOrEmpty(name)) name = "The Coroutine who must not be named.";
				gameObject = Managers.Coroutine.GameObject; 
			}

			var coroutine = gameObject.AddComponent<Coroutine>();
			coroutine._coroutine = Managers.Coroutine.CreateCoroutine(iEnumerator, gameObject);
			coroutine._coroutine.Finished += coroutine.OnCoroutineFinished;
			coroutine.name = string.IsNullOrEmpty(name) ? gameObject.name : name;

			if(start) coroutine.Run();
			return coroutine;
		}

		public static IEnumerator ActionToIEnumerator(Action a) { a(); yield return null; }

		public static IEnumerator ActionToDelayedIEnumerator(Action a, float delay) {
			yield return new WaitForSeconds(delay);
			a();
			yield return null;
		}

		public static IEnumerator ActionToUpdateIEnumerator(Action a, float updateTime) {
			while(true) {
				a();
				yield return updateTime <= 0 ? null : new WaitForSeconds(updateTime);
			}
		}

		public void Run() {
			if(Running) {
				Debug.LogError("Coroutine already running.");
			} else {
				_coroutine.Start();
			}
		}

		public void Stop() {
			if(Running) {
				_coroutine.Stop();
			}
		}

		new void OnDestroy() {
			Stop();
		}

		// Stopped is true if and only if the coroutine was stopped with an explicit call to Stop().
		void OnCoroutineFinished(bool stopped) {
			var handler = OnFinished;
			if(handler != null)	
				handler(stopped);
			Destroy(this);
		}

	}

}