using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion.StateMachine {
	[Serializable]
	public class FiniteStateMachine : MonoBehaviour {

		public State initialState;

		private State _currentState;

		private Stack<State> _stateStack;

		public void GoTo(State state) {
			if(StateStack.Count > 0)
				Debug.LogException(new Exception("Cannot switch states while the state stack is not empty."), this);
			var exitIterator = CurrentState == null ? null : CurrentState.transform;
			while(exitIterator != null && exitIterator != transform.parent && !state.transform.IsChildOf(exitIterator)) {
				var exitState = exitIterator.GetComponent<State>();
				if(exitState != null)
					exitState.OnExit();
				exitIterator.gameObject.SetActive(false);
				exitIterator = exitIterator.transform.parent;
			}

			if(exitIterator == null) exitIterator = transform;

			//iterate upwards to find the topmost common parent, then enter the states in hierarchical order
			var entryStack = new Stack<Transform>();
			entryStack.Push(state.transform);
			while(entryStack.Peek() != exitIterator) {
				entryStack.Push(entryStack.Peek().transform.parent);
			}
			while(entryStack.Count > 0) {
				var entryState = entryStack.Peek().GetComponent<State>();
				if(entryState != null) 
					entryState.OnEntry();
				entryStack.Peek().gameObject.SetActive(true);
				entryStack.Pop();
			}

			//deactivate any states in the underlying hierarchy below this one
			foreach(var childState in transform.GetComponentsInChildren<State>()) {
				if(childState == state) continue;
				childState.gameObject.SetActive(false);
			}

			_currentState = state;
		}

		//TODO deactivate the current state when the stack is not empty
		public void PushState(State state) {
			StateStack.Push(state);
			state.OnEntry();
			state.gameObject.SetActive(true);
		}

		//TODO activate the current state when the stack is empty
		public void PopState() {
			if(StateStack.Count == 0) throw new Exception("The state stack is empty. Cannot pop.");
			StateStack.Peek().OnExit();
			StateStack.Peek().gameObject.SetActive(false);
			StateStack.Pop();
		}

		private Stack<State> StateStack {
			get { return _stateStack ?? (_stateStack = new Stack<State>()); }
		}

		public State CurrentState { get { return StateStack.Count == 0 ? _currentState : StateStack.Peek(); } }
	}
}


