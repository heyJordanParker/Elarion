using System;
using System.Collections;
using System.Linq;
using Livity.Features.LiveCoding;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion {
	public class ExtendedBehaviour : MonoBehaviour {

		//TODO : Copyable component - extends this behaviour and has the OnAwakeOriginal and OnDestroyOriginal methods seen here also OnCopyAwake OnDestroyCopy
		//TODO : Also the Copyable component should include pooling logic

		//TODO : Session Variable component - registers and unregisters from the session ( aka global cache )

//		public void OnDestroy() {
//			Singleton.Get<EventManager>().Unsubscribe(this);
//		}

		//TODO override GetComponent; Cache components

		//TODO override SendMessage & Invoke; cache methods

		//TODO EventListener Component
		//Global Listener in the Event manager
		
		//Event component which should handle those operations
		public void Fire(string firedEvent) {
			Singleton.Get<EventManager>().Fire(firedEvent);
		}

		public void Broadcast(string broadcastedEvent) {
			Singleton.Get<EventManager>().Broadcast(broadcastedEvent);
		}

		public new Coroutine StartCoroutine(IEnumerator coroutine) {
			return Coroutine.Create(gameObject, coroutine);
		}

		public void Subscribe(string onEvent, string executeMethod) {
			Singleton.Get<EventManager>().Subscribe(this, onEvent, executeMethod);
		}

		// TODO Fire to all children ( send message analogue )

		public void Fire(string firedEvent, object parameter) {
			Singleton.Get<EventManager>().Fire(firedEvent, parameter);
		}

		public void Broadcast(string broadcastedEvent, object parameter) {
			Singleton.Get<EventManager>().Broadcast(broadcastedEvent, parameter);
		}
	}
}