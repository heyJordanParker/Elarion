using System.Collections;
using UnityEngine;

namespace Elarion {
	//gameFSM subscribes to LoadLevel event and pushes the state
	//gameFSM sends an event with the level to load ( this automatically subscribes to that )
	//gameFSM subscribes to FinishedLoading event and pops the state


	public class LoadingManager : ExtendedBehaviour {

		public float preLoadingDelay = 0.5f;
		public float postLoadingDelay = 1f;

		private Coroutine _loadingCoroutine;
		private LoadingProgress _loadingProgress;

		protected override void Initialize() {
			_loadingProgress = Component<LoadingProgress>();
			Managers.Resources.Add(_loadingProgress);
			Subscribe("Start Loading", "StartLoading");
		}

		protected override void Deinitialize() {
			Managers.Resources.Remove(_loadingProgress);
			if(_loadingCoroutine == null) return;
			_loadingCoroutine.Stop();
			_loadingCoroutine = null;
		}

		private void StartLoading() {
			_loadingCoroutine = StartCoroutine(LoadingCoroutine());
		}

		public IEnumerator LoadingCoroutine() {

			if(AsyncLoad.AsyncLoadIndex.Count > 0)
				Broadcast("Started Loading");
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

			Broadcast("Finished Loading");
		}

	}
}