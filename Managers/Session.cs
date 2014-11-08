
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTest;
using Object = UnityEngine.Object;

namespace Elarion.Managers {

	public static class Session {

		//mirror playerprefs
		//save to and load from player prefs
		//auto save
		//flag items to disable auto save
		//call the cleanup function
		//static access to resources

		private static WeakValueDictionary<int, Object> _index;

		public static WeakValueDictionary<int, Object> Index {
			get {
				if(_index == null)
					_index = new WeakValueDictionary<int, Object>();
				return _index;
			}
		}


		public static T GetCached<T>(int instanceId) where T : Object {
			Object result;
			if(Index.TryGetValue(instanceId, out result))
				return result as T;
			return null;
		}

		public static int Cache(Object cachedObject) {
			if(cachedObject == null) throw new ArgumentNullException("cachedObject");
			int id = cachedObject.GetInstanceID();
			if(!Index.ContainsKey(id)) {
				Index.Add(cachedObject.GetInstanceID(), cachedObject);
			}
			return id;
		}

		public static void Uncache(Object cachedObject) {
			if(cachedObject == null) throw new ArgumentNullException("cachedObject");
			int id = cachedObject.GetInstanceID();
			if(!Index.ContainsKey(id))
				throw new IndexOutOfRangeException("cachedObject is not in the Session cache.");
			Index.Remove(id);
		}

		public static void Cleanup() {
			for(int i = 0; i < Index.Count; ++i) {
				if(Index[i] != null) continue;
				Index.Remove(i--);
			}
		}
	}

}