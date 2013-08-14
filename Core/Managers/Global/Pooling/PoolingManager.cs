using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion.CoreManagers {

	public class PoolingManager : ExtendedBehaviour {

		private class PooledGameObject {

			public readonly GameObject original;
			public readonly int layer;
			public Stack<GameObject> pooled; 
			public List<GameObject> spawned;

			public PooledGameObject(GameObject original) {
				this.original = original;
				layer = original.layer;
				pooled = new Stack<GameObject>();
				spawned = new List<GameObject>();
				Prepare(original);
			}

			public void PoolOne() {
				var copy = Object.Instantiate(original) as GameObject;
				pooled.Push(copy);
				Managers.Pooling.Copies.Add(copy.GetInstanceID(), original.GetInstanceID());
			}

			public GameObject CreateOne() {
				if(pooled.Count == 0) PoolOne();
				GameObject spawn = pooled.Pop();
				spawned.Add(spawn);
				return spawn;
			}

			public void Prepare(GameObject gameObject) {
				gameObject.SetActive(false);
			}

			public GameObject Spawn(GameObject spawn) { return Spawn(spawn, -1); }

			public GameObject Spawn(GameObject spawn, int layer) {
				if(layer != -1) spawn.SetLayer(layer);
				spawn.SetActive(true);
				return spawn;
			}

			public void Destroy(GameObject gameObject) {
				spawned.Remove(gameObject);
				pooled.Push(gameObject);
			}

			public void Empty() {
				original.layer = layer;
				while(spawned.Count > 0) {
					Object.Destroy(spawned[0]);
					spawned.RemoveAt(0);
				}
				while(pooled.Count != 0) {
					Object.Destroy(pooled.Peek());
					pooled.Pop();
				}
			}

		}

		private Dictionary<int, PooledGameObject> _pools; //id object
		private Dictionary<int, int> _copies; //copy id , original id

		public GameObject Instantiate(GameObject gameObject) { return Create(gameObject, Vector3.zero, Quaternion.identity, -1); }
		public GameObject Instantiate(GameObject gameObject, int layer) { return Create(gameObject, Vector3.zero, Quaternion.identity, layer); }
		public GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation) { return Create(gameObject, position, rotation, -1); }
		public GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation, int layer) { return Create(gameObject, position, rotation, layer); }

		public Component Instantiate(Component component) {
			return Instantiate(component.gameObject).GetComponent(component.GetType());
		}
		public Component Instantiate(Component component, Vector3 position, Quaternion rotation) {
			return Instantiate(component.gameObject, position, rotation).GetComponent(component.GetType());
		}
		public Component Instantiate(Component component, Vector3 position, Quaternion rotation, int layer) {
			return Instantiate(component.gameObject, position, rotation, layer).GetComponent(component.GetType());
		}

		public GameObject Create(GameObject gameObject, Vector3 position, Quaternion rotation, int layer) {
			var original = Pools[GetOriginalID(gameObject)];
			if(layer == -1) layer = original.layer;
			var spawn = original.CreateOne();
			spawn.transform.position = position;
			spawn.transform.rotation = rotation;
			spawn.layer = layer;
			return spawn;
		}
		
		public new void Destroy(GameObject gameObject) {
			if(gameObject == null) return;
			var originalID = GetOriginalID(gameObject);
			if(originalID != gameObject.GetInstanceID()) Pools[originalID].Destroy(gameObject);
			else Object.Destroy(gameObject); //the object will take care to clean up the pool
		}

		public void Pool(GameObject gameObject, uint amount) { 
			int id = GetOriginalID(gameObject);

			for(var i = 0; i < amount; ++i) {
				Pools[id].PoolOne();
			}
		}

		public void Spawn(GameObject gameObject) {
			Pools[GetOriginalID(gameObject)].Spawn(gameObject);
		}

		public void Spawn(GameObject gameObject, int layer) {
			Pools[GetOriginalID(gameObject)].Spawn(gameObject, layer);
		}

		private int GetOriginalID(GameObject gameObject) {
			if(gameObject == null) Debug.LogException(new Exception("The spawned object is null."));
			int id = gameObject.GetInstanceID();
			if(Copies.ContainsKey(id)) id = Copies[id];
			if(!Pools.ContainsKey(id)) Pools.Add(id, new PooledGameObject(gameObject));
			return id;
		}

		public void EmptyPool(GameObject original) {
			var id = original.GetInstanceID();
			if(!Pools.ContainsKey(id)) return;
			Pools[id].Empty();
			Pools.Remove(id);
		}

		private Dictionary<int, PooledGameObject> Pools {
			get {
				if(_pools == null) _pools = new Dictionary<int, PooledGameObject>();
				return _pools;
			}
		}
		private Dictionary<int, int> Copies {
			get {
				if(_copies == null) _copies = new Dictionary<int, int>();
				return _copies;
			}
		}

	}

}