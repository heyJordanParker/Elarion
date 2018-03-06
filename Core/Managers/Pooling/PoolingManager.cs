using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion.Managers {
	public class PoolingManager : Singleton.Singleton {
		public Dictionary<int, Pool> Pools { get; private set; }

		public Pool Pool(GameObject original, uint amount) {
			if(original == null) throw new ArgumentNullException("original");

			int id = Session.Cache(original);

			Pool pool;
			if(!Pools.ContainsKey(id)) {
				pool = new Pool(original);
				Pools.Add(id, pool);
			} else {
				pool = Pools[id];
			}

			pool.Add(amount);
			return pool;
		}

		public GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation, int layer = -1) {
			if(gameObject == null) throw new ArgumentNullException("gameObject");
			int id = gameObject.GetInstanceID();
			if(!Pools.ContainsKey(id))
				throw new IndexOutOfRangeException("The GameObject is not pooled.");
			return Pools[id].Spawn(position, rotation, layer);
		}

		new void Awake() {
			base.Awake();
			Pools = new Dictionary<int, Pool>();
		}
	}
}