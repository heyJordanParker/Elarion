using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elarion.CoreManagers {

	public class ResourcesManager : ExtendedBehaviour {

		private List<Object> _resources;

		public T Get<T>() where T : Object {
			return (from resource in Resources where typeof(T) == resource.GetType() select resource as T).FirstOrDefault();
		}

		public void Add(Object resource) {
			Resources.Add(resource);
		}

		public void Remove(Object resource) {
			Resources.Remove(resource);
		}

		private List<Object> Resources { get { return _resources == null ? (_resources = new List<Object>()) : _resources; } }
	}

}

