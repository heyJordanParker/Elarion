using System;
using System.Collections;
using UnityEngine;

namespace Elarion {

	public class AsyncSceneLoader : AsyncLoad {

		public string loadLevelName;

		protected override IEnumerator Load() {
			if(string.IsNullOrEmpty(loadLevelName) || Application.loadedLevelName == loadLevelName) yield break;

			var loading = Application.LoadLevelAsync(loadLevelName);

			while(!loading.isDone) {
				yield return null;
			}

			yield return null;

			GC.Collect();

			yield return null;
		}
	}

}