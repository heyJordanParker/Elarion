using System.Collections;
using Elarion.Managers;
using Elarion.StateMachine;
using UnityEngine;

namespace Elarion {

	public sealed class GameFSM : FiniteStateMachine {

		public Database[] databases;
		
		//TODO game state loading should go as follows : Exit Old State, Enter New State, Fire(Initialize) on New State, Start Loading Coroutine on new state
		//	( intended behavior - an empty scene for initialization than loading of the starting level etc ) 

		//data for following of previous levels is stored in the Session

		//TODO subscribe to a change level event - push the loading etc

		private void Awake() {
			foreach(var database in databases) {
				database.Initialize();
//				Session.Add(database);
			}
			gameObject.Subscribe("Change Game State", "GoToGameState");
		}

		private void OnDestroy() {
			foreach(var database in databases) {
				database.Deinitialize();
//				Session.Remove(database);
			}
		}

		public void GoToGameState(GameState gameState) {
			GoTo(gameState);
		}

//		public static void GoTo(GameState gameState, string scene) {
//
//			Instance._gameState = state;
////			Instance.FSM.GoTo(state);
//
////			state.OnEnter();
////			Instance.FSM.PushState(GameStateEnum.Loading);
//			Managers.Loading.StartLoading(Instance._loadingScene, state, scene);
//		}

	}

}

