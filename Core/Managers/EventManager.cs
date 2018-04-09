using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elarion.Managers {

	internal class EventManager : Singleton.SingletonBehaviour {
		private static List<EventHandler> _eventHandlersIndex;

		private static List<EventHandler> EventHandlersIndex {
			get { return _eventHandlersIndex == null ? (_eventHandlersIndex = new List<EventHandler>()) : _eventHandlersIndex; }
		}

		public void Fire(GameObject target, string firedEvent, GameObject sender) {
			Fire(target, firedEvent, new EventArguments(sender));
		}

		public void Fire(GameObject target, string firedEvent, EventArguments eventArguments) {
			var eventHandler = EventHandlersIndex.FirstOrDefault(e => e.gameObject == target);
			if(eventHandler == null) return;
			eventHandler.Fire(firedEvent, eventArguments);
		}

		public void Broadcast(string broadcastedEvent, GameObject sender) {
			Broadcast(broadcastedEvent, new EventArguments(sender));
		}

		public void Broadcast(string broadcastedEvent, EventArguments eventArguments) {
			foreach(var eventHandler in EventHandlersIndex) {
				eventHandler.Fire(broadcastedEvent, eventArguments);
			}
		}

		public void Register(EventHandler eventHandler) {
			EventHandlersIndex.Add(eventHandler);
		}

		public void Unregister(EventHandler eventHandler) {
			EventHandlersIndex.Remove(eventHandler);
		}
	}

}