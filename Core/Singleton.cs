using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elarion {

	public class Singleton : ExtendedBehaviour {

		private static Dictionary<Type, GameObject> _instances;

		private static Dictionary<Type, GameObject> Instances {
			get {
				if(_instances == null)
					_instances = new Dictionary<Type, GameObject>();
				return _instances;
			}
		}

		protected new void Awake() {
			var type = GetType();
			GameObject instance;
			if(!Instances.TryGetValue(type, out instance)) {
				Instances.Add(type, gameObject);
				base.Awake();
			} else if(instance.GetInstanceID() != gameObject.GetInstanceID()) {
				Debug.Log("Destroying GameObject " + gameObject.name + " because an instance of this Singleton already exists.", instance);
				Destroy(gameObject);
			}
		}

	}

}
