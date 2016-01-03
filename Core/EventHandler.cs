using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion.EventSystem {
	public sealed class EventHandler : MonoBehaviour {

		private Dictionary<string, List<string>> _events;

		public void Awake() {
			EventManager.Register(this);
		}

		public void OnDestroy() {
			EventManager.Unregister(this);
		}

		private static EventManager EventManager { get { return Singleton.Get<EventManager>(); } }

		public void Subscribe(string toEvent, string message) {
			if(!Events.ContainsKey(toEvent))
				Events.Add(toEvent, new List<string>());
			Events[toEvent].Add(message);
		}

		public void Unsubscribe(string fromEvent) {
			if(Events.ContainsKey(fromEvent))
				Events.Remove(fromEvent);
		}

		public void Broadcast(string firedEvent, GameObject sender) {
			Broadcast(firedEvent, new EventArguments(sender));
		}

		public void Broadcast(string firedEvent, EventArguments eventArguments) {
			EventManager.Broadcast(firedEvent, eventArguments);
		}

		public void Fire(string firedEvent, GameObject sender) {
			Fire(firedEvent, new EventArguments(sender));
		}

		public void Fire(string firedEvent, EventArguments eventArguments) {
			List<string> messagesToSend;
			if(!Events.TryGetValue(firedEvent, out messagesToSend))
				return;
			foreach(var message in messagesToSend)
				gameObject.SendMessage(message, eventArguments, SendMessageOptions.RequireReceiver);
		}

		internal void FireInternal(string firedEvent, object parameter) {
			
		}
		private Dictionary<string, List<string>> Events {
			get {
				if(_events == null)
					_events = new Dictionary<string, List<string>>();
				return _events;
			}
		}
	}

}