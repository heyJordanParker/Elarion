using System;
using UnityEngine;

namespace Elarion.StateMachine {

	[Serializable]
	public class State : MonoBehaviour {

		public virtual void OnEntry() { }

		public virtual void OnExit() { }

		public State Parent {
			get {
				State state = null;
				Transform current = transform.parent;
				while(current != null && (state = current.GetComponent<State>()) == null) {
					if(current.GetComponent<FiniteStateMachine>() != null) break; 
					current = current.parent;
				}
				return state;
			}
		}

	}
}
