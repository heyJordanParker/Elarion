using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnEnableHandler : MonoBehaviour {
        public UnityEvent onEnable;

        public event Action OnEnableEvent = () => { };

        protected virtual void OnEnable() {
            OnEnableEvent();
            onEnable.Invoke();
        }
    }
}