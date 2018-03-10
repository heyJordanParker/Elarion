using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion.Managers {

	[Serializable]
	public class Pool {
		[SerializeField] private GameObject _original;

		private List<Pooled> _pooledObjects;

		//auto extend ( aka if you try to spawn an object that already has all of it's instances in use, spawn another instance or pop an error )
		//a list of those in the pooling manager
		//methods for extracting objects and deleting them
		//Loading interface

		public int Count {
			get { return PooledObjects.Count; }
		}

		public GameObject Original { get { return _original; } private set { _original = value; }}

		public int Spawned {
			get { return PooledObjects.Count(pooled => pooled.IsSpawned); }
		}

		public List<Pooled> PooledObjects { get { return _pooledObjects; } private set { _pooledObjects = value; } }

		public Pool(GameObject original) {
			if(original == null) throw new ArgumentNullException("original");

			PooledObjects = new List<Pooled>();
			Original = original;
		}

		public void Clear() {
			while(PooledObjects.Count > 0) {
				var pooled = PooledObjects[0];
				if(pooled.IsSpawned)
					Despawn(pooled);
				pooled.gameObject.Destroy();
				PooledObjects.Remove(pooled);
			}
		}

		public IEnumerator<Pooled> GetEnumerator() {
			return PooledObjects.GetEnumerator();
		}

		public void Add(uint amount) {
			for(var i = 0; i < amount; ++i) {
				AddOne();
			}
		}

		public Pooled AddOne() {
			var copy = Copy(Original);
			PooledObjects.Add(copy);
			copy.pool = this;
			copy.gameObject.SetActive(false);
			return copy;
		}

		private Pooled Copy(GameObject gameObject) {
			var copy = Object.Instantiate(gameObject);
			return copy.AddComponent<Pooled>();
		}

		public bool Contains(Pooled pooledObject) {
			if(pooledObject == null) throw new ArgumentNullException("pooledObject");
			return PooledObjects.Contains(pooledObject);
		}

		public GameObject Spawn(Vector3 position = new Vector3(), Quaternion rotation = new Quaternion(), int layer = -1) {
			if(layer == -1) {
				layer = Original.layer;
			}

			var spawningObject = PooledObjects.FirstOrDefault(pooled => !pooled.IsSpawned) ?? AddOne();
			spawningObject.transform.position = position;
			spawningObject.transform.rotation = rotation;
			spawningObject.gameObject.layer = layer;
			spawningObject.gameObject.SetActive(true);
			spawningObject.SendMessage(Pooled.OnSpawnMethod, SendMessageOptions.DontRequireReceiver);
			return spawningObject.gameObject;
		}


		public void Despawn(GameObject poolObject) {
			Despawn(poolObject.GetComponent<Pooled>());
			
		}

		public void Despawn(Pooled pooled) {
			pooled.SendMessage(Pooled.OnDespawnMethod, SendMessageOptions.DontRequireReceiver);
		}
	}
}