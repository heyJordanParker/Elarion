using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Workflows.EventListeners.DefaultEventListeners {
    public class DisableEventListener : MonoBehaviour {
        public UnityEvent onDisable;

        public event Action OnDisableEvent = () => { };

        protected virtual void OnDisable() {
            OnDisableEvent();
            onDisable.Invoke();
        }
    }
}