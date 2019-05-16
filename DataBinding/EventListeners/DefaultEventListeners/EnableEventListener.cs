using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.DataBinding.EventListeners.DefaultEventListeners {
    public class EnableEventListener : MonoBehaviour {
        public UnityEvent onEnable;

        public event Action OnEnableEvent = () => { };

        protected virtual void OnEnable() {
            OnEnableEvent();
            onEnable.Invoke();
        }
    }
}