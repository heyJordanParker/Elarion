using Elarion.StateMachine;
using UnityEngine;

namespace Elarion {

	public class GameState : State {

		public bool startLoading;

		public override void OnEntry() {
			if(startLoading)
				gameObject.Broadcast("Start Loading");
		}

	}
}