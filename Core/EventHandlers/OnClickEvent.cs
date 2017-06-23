using Elarion.Extensions;
using UnityEngine;

namespace Elarion {

	public class OnClickEvent : MonoBehaviour {
		 
		public string firedEvent;
		
		void OnClick() {
			if(string.IsNullOrEmpty(firedEvent)) return;
			
			gameObject.Broadcast(firedEvent);
		}

	}

}