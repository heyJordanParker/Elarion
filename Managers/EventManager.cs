using System.Collections.Generic;
using UnityEngine;

namespace Elarion {

	internal class EventManager : Singleton {
		
		private struct Event {
			private string _name;
			private object _parameter;
			private GameObject _target;

			public Event(string name, object parameter, GameObject target) {
				_name = name;
				_parameter = parameter;
				_target = target;
			}

			public string Name { get { return _name; } }
			public object Parameter { get { return _parameter; } }
			public GameObject Target { get { return _target; } }
		}

		private static List<EventHandler> _eventHandlersIndex;
		private static Stack<Event> _fireStack;

		internal void Subscribe(ExtendedBehaviour behaviour, string subscribeToEvent, string messageToSend) {
			var eventHandler = EventHandlersIndex.Find(hander => hander.GameObject == behaviour.gameObject);
			if(eventHandler == null) {
				eventHandler = new EventHandler(behaviour.gameObject);
				EventHandlersIndex.Add(eventHandler);
			}
			eventHandler.Subscribe(behaviour, subscribeToEvent, messageToSend);
		}

		internal void Unsubscribe(ExtendedBehaviour behaviour) {
			var eventHandler = EventHandlersIndex.Find(hander => hander.GameObject == behaviour.gameObject);
			if(eventHandler == null) return;

			eventHandler.Unsubscribe(behaviour);

			if(eventHandler.Subscribers == 0)
				EventHandlersIndex.Remove(eventHandler);
		}

		internal new void Broadcast(string broadcastedEvent) {
			Fire(broadcastedEvent);
		}

		internal new void Broadcast(string broadcastedEvent, object parameter) {
			Fire(broadcastedEvent, parameter);
		}

		internal new void Fire(string broadcastedEvent) {
			Fire(broadcastedEvent, null);
		}

		internal new void Fire(string broadcastedEvent, object parameter) {
			FireStack.Push(new Event(broadcastedEvent, parameter, null));
		}

		internal void Fire(string firedEvent, GameObject target) { Fire(firedEvent, target, null); }

		internal void Fire(string firedEvent, GameObject target, object parameter) {
			FireStack.Push(new Event(firedEvent, parameter, target));
		}

		void Update() {
			while(FireStack.Count > 0) {
				ProcessEvent(FireStack.Peek());
				FireStack.Pop();
			}
		}

		private static void ProcessEvent(Event eEvent) {
			if(eEvent.Target == null) {
				foreach(var eventHandler in EventHandlersIndex) {
					eventHandler.FireInternal(eEvent.Name, eEvent.Parameter);
				}
			} else {
				var eventHandler = EventHandlersIndex.Find(hander => hander.GameObject == eEvent.Target);
				if(eventHandler == null) return;
				eventHandler.FireInternal(eEvent.Name, eEvent.Parameter);
			}
		}

		private static Stack<Event> FireStack {
			get { return _fireStack == null ? (_fireStack = new Stack<Event>()) : _fireStack; }
		}

		private static List<EventHandler> EventHandlersIndex {
			get { return _eventHandlersIndex == null ? (_eventHandlersIndex = new List<EventHandler>()) : _eventHandlersIndex; }
		}
	}

}