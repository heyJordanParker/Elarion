using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.DataBinding.EventListeners.DefaultEventListeners {
    public class DestroyEventListener : MonoBehaviour {
        public UnityEvent onDestroy;

        public event Action OnDestroyEvent = () => { };

        protected virtual void OnDestroy() {
            OnDestroyEvent();
            onDestroy.Invoke();
        }
    }
}