namespace Elarion {

	public class OnClickEvent : ExtendedBehaviour {
		 
		public string firedEvent;
		public bool sendSelfReference;
		
		void OnClick() {
			if(string.IsNullOrEmpty(firedEvent)) return;
			
			
			if(sendSelfReference)
				Broadcast(firedEvent, gameObject);
			else
				Broadcast(firedEvent);
		}

	}

}