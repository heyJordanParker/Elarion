using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnStartHandler : MonoBehaviour {
        public UnityEvent onStart;

        public event Action OnStartEvent = () => { };

        protected virtual void Start() {
            OnStartEvent();
            onStart.Invoke();
        }
    }
}