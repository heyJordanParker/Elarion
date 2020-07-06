using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Workflows.EventListeners.DefaultEventListeners {
    public class LateUpdateEventListener : MonoBehaviour {
        public UnityEvent onLateUpdate;

        public event Action OnLateUpdateEvent = () => { };

        protected virtual void LateUpdate() {
            OnLateUpdateEvent();
            onLateUpdate.Invoke();
        }
    }
}