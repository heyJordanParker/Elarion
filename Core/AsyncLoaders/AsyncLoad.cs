using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion {

	public class AsyncLoad : MonoBehaviour {

		//set this to true when the loading has finished
		private bool _isDone;
		private bool _started;

		private static List<AsyncLoad> _asyncLoadIndex;

		protected void OnEnable() {
			_isDone = false;
			_started = false;
			AsyncLoadIndex.Add(this);
		}

		protected void OnDisable() {
			AsyncLoadIndex.Remove(this);
		}

		internal void StartLoading() {
			_started = true;
			gameObject.StartCoroutine(LoadWrapper());
		}

		private IEnumerator LoadWrapper() {
			yield return Load();
			_isDone = true;
		}

		protected virtual IEnumerator Load() {
			yield return null;
		}

		public bool IsDone { get { return _isDone; } }
		public bool Started { get { return _started; } }
		internal static List<AsyncLoad> AsyncLoadIndex { get { return _asyncLoadIndex == null ? (_asyncLoadIndex = new List<AsyncLoad>()) : _asyncLoadIndex; } }
	}

}