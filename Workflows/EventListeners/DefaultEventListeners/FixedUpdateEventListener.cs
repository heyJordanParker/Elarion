using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Workflows.EventListeners.DefaultEventListeners {
    public class FixedUpdateEventListener : MonoBehaviour {
        public UnityEvent onFixedUpdate;

        public event Action OnFixedUpdateEvent = () => { };

        protected virtual void FixedUpdate() {
            OnFixedUpdateEvent();
            onFixedUpdate.Invoke();
        }
    }
}