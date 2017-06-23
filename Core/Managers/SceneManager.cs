using System.Collections;
using Elarion.Extensions;
using UnityEngine;

namespace Elarion {
	//gameFSM subscribes to LoadLevel event and pushes the state
	//gameFSM sends an event with the level to load ( this automatically subscribes to that )
	//gameFSM subscribes to FinishedLoading event and pops the state

	[RequireComponent(typeof(LoadingProgress))]
	public class SceneManager : MonoBehaviour {

		public float preLoadingDelay = 0.5f;
		public float postLoadingDelay = 1f;

		private LoadingProgress _loadingProgress;

		void Awake() {
			_loadingProgress = new LoadingProgress();
		}

		private void StartLoading() {
			StartCoroutine(LoadingCoroutine());
		}

		public IEnumerator LoadingCoroutine() {

			if(AsyncLoad.AsyncLoadIndex.Count > 0)
				gameObject.Broadcast("Started Loading");
			else yield break;

			yield return new WaitForSeconds(preLoadingDelay);

			while(true) {
				var finishedLoaders = 0;
				foreach(var asyncLoad in AsyncLoad.AsyncLoadIndex) {
					if(!asyncLoad.Started) asyncLoad.StartLoading();
					if(asyncLoad.IsDone) ++finishedLoaders;
				}
				_loadingProgress.progress = finishedLoaders / (float) AsyncLoad.AsyncLoadIndex.Count;
				if(finishedLoaders == AsyncLoad.AsyncLoadIndex.Count) break;
				yield return null;
			}

			yield return new WaitForSeconds(postLoadingDelay);

			gameObject.Broadcast("Finished Loading");
		}

	}
}