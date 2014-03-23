using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion {

	internal sealed class EventHandler {
		private bool Equals(EventHandler other) {
			return GameObject.Equals(other.GameObject);
		}

		//events & list of methods to call on every event
		private Dictionary<string, List<string>> _events;
		private List<ExtendedBehaviour> _subscribersIndex; 

		private readonly GameObject _gameObject;

		public EventHandler(GameObject gameObject) { _gameObject = gameObject; }

		public void Subscribe(ExtendedBehaviour behaviour, string toEvent, string messageToSend) {
			SubscribersIndex.Add(behaviour);
			if(!Events.ContainsKey(toEvent))
				Events.Add(toEvent, new List<string>());
			Events[toEvent].Add(messageToSend);
		}

		public void Unsubscribe(ExtendedBehaviour behaviour) { SubscribersIndex.Remove(behaviour); }

		internal void FireInternal(string firedEvent, object parameter) {
			List<string> messagesToSend;
			if(!Events.TryGetValue(firedEvent, out messagesToSend)) return;
			foreach(var message in messagesToSend)
				GameObject.SendMessage(message, parameter, SendMessageOptions.RequireReceiver);
		}

		private Dictionary<string, List<string>> Events {
			get {
				if(_events == null) 
					_events = new Dictionary<string, List<string>>(); 
				return _events;
			}
		}

		private List<ExtendedBehaviour> SubscribersIndex {
			get {
				if(_subscribersIndex == null)
					_subscribersIndex = new List<ExtendedBehaviour>();
				return _subscribersIndex;
			}
		}

		public GameObject GameObject { get { return _gameObject; } }

		public int Subscribers { get { return SubscribersIndex.Count; } }

		public override bool Equals(object obj) {
			if(ReferenceEquals(null, obj))
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			return obj is EventHandler && Equals((EventHandler)obj);
		}

		public override int GetHashCode() {
			return GameObject.GetHashCode();
		}

		public static bool operator ==(EventHandler left, EventHandler right) { return Equals(left, right); }
		public static bool operator !=(EventHandler left, EventHandler right) { return !Equals(left, right); }

	}

}