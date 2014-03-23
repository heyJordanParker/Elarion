using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion {

	//TODO test how nested Breaks work ( if they do )

	/// <summary>
	/// Internal class to help managing coroutines
	/// Managers.CoroutineSchedule
	/// </summary>
	internal class CoroutineInternal {

		private enum CoroutineResult {
			Running,
			Finished,
		}

		internal event Action<bool> Finished;

		private IEnumerator _coroutine;
		private bool _paused;
		private bool _stopped;
		private GameObject _gameObject;

		internal bool Running { get; private set; }
		internal bool Paused { get { return _paused; } set { _paused = value; } }

		internal CoroutineInternal(IEnumerator c, GameObject gameObject) {
			_coroutine = c;
			_gameObject = gameObject;
		}

		internal void Start() {
			Running = true;
			Managers.Coroutine.StartCoroutine(CallWrapper());
		}

		internal void Stop() {
			_stopped = true;
			Running = false;
		}

		IEnumerator CallWrapper() {
			yield return null;
			var e = _coroutine;
			while(Running) {
				if(_gameObject == null)
					yield break;
				if(_paused || !_gameObject.activeInHierarchy)
					yield return null;
				else {
					object result;
					if(ProcessEnumerator(e, out result, false) == CoroutineResult.Finished) {
						Running = false;
					} else yield return result;
//					if(e != null && e.MoveNext()) {
//						yield return e.Current;
//					} else {
//						Running = false;
//					}
				}
			}

			var handler = Finished;
			if(handler != null)
				handler(_stopped);
		}

		CoroutineResult ProcessEnumerator(IEnumerator e, out object result, bool finishedInner) {
			if(e == null) {
				result = null;
				return CoroutineResult.Finished;
			}
			if((e.Current as IEnumerator) == null) {
				if(finishedInner || e.MoveNext()) {
					result = e.Current;
					return CoroutineResult.Running;
				}
				result = null;
				return CoroutineResult.Finished;
			}
			var inner = e.Current as IEnumerator;
			if(ProcessEnumerator(inner, out result, false) != CoroutineResult.Finished) {
				return CoroutineResult.Running;
			}
			if(e.MoveNext()) return ProcessEnumerator(e, out result, true);
			result = null;
			return CoroutineResult.Finished;
		}
	}
}