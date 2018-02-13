using Elarion.Managers;
using UnityEngine;

namespace Elarion.Extensions {
	public static class GameObjectExtensions {

		public static T Component<T>(this GameObject go) where T : Component {
			var component = go.GetComponent<T>();
			if(component == null)
				component = go.AddComponent<T>();
			return component;
		}


		public static void Subscribe(this GameObject go, string toEvent, string message) {
			var eventHandler = go.Component<EventHandler>();
			eventHandler.Subscribe(toEvent, message);
		}

		public static void Unsubscribe(this GameObject go, string fromEvent) {
			var eventHandler = go.GetComponent<EventHandler>();
			if(eventHandler == null) {
				Debug.LogError(string.Format("Trying to unsubscribe from event {0} but the GameObject doesn't have an EventHandler component", fromEvent), go);
				return;
			}
			eventHandler.Unsubscribe(fromEvent);
		}

		public static void Fire(this GameObject go, string firedEvent) {
			var eventHandler = go.GetComponent<EventHandler>();
			eventHandler.Fire(firedEvent, go);			
		}

		public static void Fire(this GameObject go, string firedEvent, EventArguments eventArguments) {
			var eventHandler = go.GetComponent<EventHandler>();
			eventHandler.Fire(firedEvent, eventArguments);			
		}

		public static void Broadcast(this GameObject go, string firedEvent) {
			var eventHandler = go.GetComponent<EventHandler>();
			eventHandler.Broadcast(firedEvent, go);
		}

		public static void Broadcast(this GameObject go, string firedEvent, EventArguments eventArguments) {
			var eventHandler = go.GetComponent<EventHandler>();
			eventHandler.Broadcast(firedEvent, eventArguments);
		}

		public static void SetLayer(this GameObject go, int layer, bool recursive = true) {
			go.layer = layer;
			if(!recursive) return;
			foreach(Transform child in go.transform) {
				SetLayer(child.gameObject, layer);
			}
		}

		public static Pool Pool(this GameObject go, uint amount) {
			var poolingManager = Singleton.Get<PoolingManager>();
			return poolingManager.Pool(go, amount);
		}

	}
}

