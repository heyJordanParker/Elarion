using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnDisableHandler : MonoBehaviour {
        public UnityEvent onDisable;

        public event Action OnDisableEvent = () => { };

        protected virtual void OnDisable() {
            OnDisableEvent();
            onDisable.Invoke();
        }
    }
}