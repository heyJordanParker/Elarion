using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion {

	public abstract class Singleton : ExtendedBehaviour {

		private static Dictionary<Type, Singleton> _instances;

		private static Dictionary<Type, Singleton> Instances {
			get {
				if(_instances == null)
					_instances = new Dictionary<Type, Singleton>();
				return _instances;
			}
		}

		public static T Get<T>() where T : Singleton {
			Singleton singleton;
			if(!Instances.TryGetValue(typeof(T), out singleton)) {
				return null;
			}
			return (T) singleton;
		}

		protected void Awake() {
			var type = GetType();
			Singleton instance;
			if(!Instances.TryGetValue(type, out instance)) {
				Instances.Add(type, this);
			} else if(instance != this) {
				Debug.Log("Destroying Singleton in GameObject " + gameObject.name + " because an instance of this Singleton already exists.", instance);
				Destroy(this);
			}
		}

		protected void OnDestroy() {
			Instances.Remove(GetType());
		}


		public static void Cleanup() {
			_instances = null;
		}
	}

}
