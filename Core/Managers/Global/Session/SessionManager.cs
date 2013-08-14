
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elarion.CoreManagers {

	public class SessionManager : ExtendedBehaviour {
		
		private Dictionary<Type, List<ExtendedBehaviour>> _behavioursLookup;

		//TODO use Events
		public event Action<ExtendedBehaviour> OnBehaviourRegistered;
		public event Action<ExtendedBehaviour> OnBehaviourRevoked;


		internal void RegisterBehaviour(ExtendedBehaviour behaviour) {
			var type = behaviour.GetType();
			if(!BehavioursLookup.ContainsKey(type)) BehavioursLookup.Add(type, new List<ExtendedBehaviour>());
			BehavioursLookup[type].Add(behaviour);
			var handler = OnBehaviourRegistered;
			if(handler != null) handler(behaviour);
		}

		internal void RevokeBehaviour(ExtendedBehaviour behaviour) {
			var type = behaviour.GetType();
			if(!BehavioursLookup.ContainsKey(type)) return;
			var handler = OnBehaviourRevoked;
			if(handler != null) handler(behaviour);
			BehavioursLookup[type].Remove(behaviour);
			if(BehavioursLookup[type].Count == 0) BehavioursLookup.Remove(type);
		}

		internal T[] GetAllBehavioursOfType<T>() where T : ExtendedBehaviour{
			var type = typeof(T);
			if(!BehavioursLookup.ContainsKey(type)) return null;
			return BehavioursLookup[type].Cast<T>().ToArray();
		}

		internal ExtendedBehaviour[] GetAllBehavioursOfType(Type type) {
			if(!BehavioursLookup.ContainsKey(type))
				return new ExtendedBehaviour[0];
			return BehavioursLookup[type].Where(e => e.gameObject.activeInHierarchy).ToArray();
		}

		internal Dictionary<Type, List<ExtendedBehaviour>> BehavioursLookup {
			get {
				if(_behavioursLookup == null)
					_behavioursLookup = new Dictionary<Type, List<ExtendedBehaviour>>();
				return _behavioursLookup;
			}
		}
	}

}