using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Workflows.EventListeners.DefaultEventListeners {
    public class ApplicationQuitEventListener : MonoBehaviour {
        public UnityEvent onApplicationQuit;

        public event Action OnApplicationQuitEvent = () => { };

        protected virtual void OnApplicationQuit() {
            OnApplicationQuitEvent();
            onApplicationQuit.Invoke();
        }
    }
}