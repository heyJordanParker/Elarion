using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.DataBinding.EventListeners.DefaultEventListeners {
    public class UpdateEventListener : MonoBehaviour {
        public UnityEvent onUpdate;

        public event Action OnUpdateEvent = () => { };

        protected virtual void Update() {
            OnUpdateEvent();
            onUpdate.Invoke();
        }
    }
}