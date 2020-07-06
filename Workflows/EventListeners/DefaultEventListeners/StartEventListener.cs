using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Workflows.EventListeners.DefaultEventListeners {
    public class StartEventListener : MonoBehaviour {
        public UnityEvent onStart;

        public event Action OnStartEvent = () => { };

        protected virtual void Start() {
            OnStartEvent();
            onStart.Invoke();
        }
    }
}