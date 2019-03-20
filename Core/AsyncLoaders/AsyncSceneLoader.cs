using System;
using System.Collections;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Elarion.AsyncLoaders {

	public class AsyncSceneLoader : AsyncLoad {

		public string loadLevelName;

		protected override IEnumerator Load() {
			if(string.IsNullOrEmpty(loadLevelName) || USceneManager.GetActiveScene().name == loadLevelName) yield break;

			var loading = USceneManager.LoadSceneAsync(loadLevelName);

			while(!loading.isDone) {
				yield return null;
			}

			yield return null;

			GC.Collect();

			yield return null;
		}
	}

}