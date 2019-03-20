using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elarion.Coroutines;
using Elarion.Utility;
using UnityEngine;

namespace Elarion.Pooling {
    // TODO custom editor with a Repopulate Pool Button
    // TODO repopulate pool if the prefab changes (maybe use hashcodes or just the reference); editor only
    [CreateAssetMenu(menuName = "Object Pool", order = 33)]
    public class ObjectPool : ScriptableObject {
        [SerializeField]
        private PooledObject _pooledObject;

        [SerializeField]
        private int _initialSize = 10;

        [SerializeField]
        [Range(1, 100)]
        private int _maxInstantiationsPerFrame = 10;

        [SerializeField]
        private int _initializationPriority = 1;

        [SerializeField]
        private bool _hideObjects;
        
        private Transform PooledObjectsContainer { get; set; }
        
        private Stack<PooledObject> PooledObjects { get; set; }

        private void Initialize(Transform containerParent) {
            PooledObjectsContainer = new GameObject(_pooledObject.name + " Pool").transform;
            PooledObjectsContainer.SetParent(containerParent);
            PooledObjects = new Stack<PooledObject>(_initialSize);
        }

        public PooledObject Spawn(Transform parent, bool autoInitialize = true) {
            if(PooledObjects.Count == 0) {
                AddObject();
            }

            var spawnedObject = PooledObjects.Pop();
            spawnedObject.transform.SetParent(parent);

            if(autoInitialize) {
                spawnedObject.Initialize();
            }
            
            return spawnedObject;
        }

        public TPooledObject Spawn<TPooledObject>(Transform parent, bool autoInitialize = true) where TPooledObject : PooledObject {
            return Spawn(parent, autoInitialize) as TPooledObject;
        }

        public PooledObject[] SpawnMultiple(int count, Transform parent, bool autoInitialize = true) {
            var results = new PooledObject[count];
            
            for(int i = 0; i < count; ++i) {
                results[i] = Spawn(parent, autoInitialize);
            }

            return results;
        }

        public void Return(PooledObject pooledObject) {
            if(pooledObject == null) {
                return;
            }
            
            pooledObject.Deinitialize();
            pooledObject.transform.SetParent(PooledObjectsContainer);
            PooledObjects.Push(pooledObject);
        }

        private void AddObject() {
            var instance = Instantiate(_pooledObject, PooledObjectsContainer);
            instance.Pool = this;
            
            if(_hideObjects) {
                instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            
            PooledObjects.Push(instance);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitializePools() {
            var pools = Resources.FindObjectsOfTypeAll<ObjectPool>();
            if(pools.Length == 0) {
                return;
            }
            
            var poolsContainer = new GameObject("Pools");
            DontDestroyOnLoad(poolsContainer);

            var executor = poolsContainer.AddComponent<EmptyBehavior>();
            
            ECoroutine e = new ECoroutine(InitializePoolsCoroutine(poolsContainer, pools), executor);
        }

        private static IEnumerator InitializePoolsCoroutine(GameObject poolsContainer, ObjectPool[] pools) {
            var sortedPools = pools.OrderByDescending(p => p._initializationPriority);

            foreach(var pool in pools) {
                pool.Initialize(poolsContainer.transform);
            }

            foreach(var pool in sortedPools) {
                for(int i = 0; i < pool._initialSize; ++i) {
                    pool.AddObject();

                    if((i + 1) % pool._maxInstantiationsPerFrame == 0) {
                        yield return null;
                    }
                }
                
                yield return null;
            }

        }
    }
}