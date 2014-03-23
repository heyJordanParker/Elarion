using Elarion.StateMachine;
using UnityEngine;

namespace Elarion {

	public class GameState : State {

		public bool startLoading;

		public override void OnEntry() {
			if(startLoading)
				Broadcast("Start Loading");
		}

	}
}