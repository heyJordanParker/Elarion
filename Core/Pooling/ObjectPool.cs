using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elarion.Extensions;
using Elarion.Utility;
using UnityEngine;

namespace Elarion.Pooling {
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

        public void Return(PooledObject pooledObject) {
            pooledObject.Deinitialize();
            pooledObject.transform.SetParent(PooledObjectsContainer);
            PooledObjects.Push(pooledObject);
        }

        private void AddObject() {
            var instance = Instantiate(_pooledObject, PooledObjectsContainer);
            instance.Pool = this;
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