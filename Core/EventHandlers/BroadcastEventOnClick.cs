using Elarion.Extensions;
using UnityEngine;

namespace Elarion {

	public class BroadcastEventOnClick : MonoBehaviour {
		 
		public string firedEvent;

		void OnClick() {
			if(string.IsNullOrEmpty(firedEvent)) return;
			gameObject.Broadcast(firedEvent);
		}

	}

}