using Elarion.Extensions;
using Elarion.StateMachine;

namespace Elarion {

	public class GameState : State {

		public bool startLoading;

		public override void OnEntry() {
			if(startLoading)
				gameObject.Broadcast("Start Loading");
		}

	}
}