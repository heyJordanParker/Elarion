namespace Elarion {

	public class BroadcastEventOnClick : ExtendedBehaviour {
		 
		public string firedEvent;

		void OnClick() {
			if(string.IsNullOrEmpty(firedEvent)) return;
			Broadcast(firedEvent);
		}

	}

}