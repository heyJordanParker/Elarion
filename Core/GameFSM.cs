using System.Collections;
using Elarion.StateMachine;
using UnityEngine;

namespace Elarion {

	public sealed class GameFSM : FiniteStateMachine {

		public Database[] databases;
		
		//TODO game state loading should go as follows : Exit Old State, Enter New State, Fire(Initialize) on New State, Start Loading Coroutine on new state
		//	( intended behavior - an empty scene for initialization than loading of the starting level etc )

		//data for following of previous levels is stored in the Session

		//TODO subscribe to a change level event - push the loading etc

		protected override void Initialize() {
			foreach(var database in databases) {
				database.Initialize();
				Managers.Resources.Add(database);
			}
			base.Initialize();
			Subscribe("Change Game State", "GoToGameState");
		}

		protected override void Deinitialize() {
			base.Deinitialize();
			foreach(var database in databases) {
				database.Deinitialize();
				Managers.Resources.Remove(database);
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

