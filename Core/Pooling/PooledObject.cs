using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Pooling {
    // TODO custom editor - events always at the end plus buttons for initializing and returning to pool
    public class PooledObject : ExtendedBehaviour {

        [SerializeField]
        private UnityEvent _onInitialize = new UnityEvent();
        [SerializeField]
        private UnityEvent _onDeinitialize = new UnityEvent();

        [SerializeField, HideInInspector]
        private ObjectPool _pool;

        private bool _initialized;

        internal ObjectPool Pool {
            get => _pool;
            set => _pool = value;
        }

        protected virtual void Awake() {
            gameObject.SetActive(false);
        }
        
        public void Initialize() {
            if(_initialized) {
                #if UNITY_EDITOR
                Debug.Log("Reinitializing an initialized pooled object.", this);
                #endif
                return;
            }
            OnInitialize();
            _onInitialize.Invoke();
            gameObject.SetActive(true);
            _initialized = true;
        }

        public void ReturnToPool() {
            Pool.Return(this); // this deinitializes to avoid duplication
        }

        internal void Deinitialize() {
            OnDeinitialize();
            gameObject.SetActive(false);
            _onDeinitialize.Invoke();
            _initialized = false;
        }

        protected virtual void OnInitialize() { }

        protected virtual void OnDeinitialize() { }
    }
}