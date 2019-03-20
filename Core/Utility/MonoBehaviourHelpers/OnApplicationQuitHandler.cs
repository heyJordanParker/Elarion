using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnApplicationQuitHandler : MonoBehaviour {
        public UnityEvent onApplicationQuit;

        public event Action OnApplicationQuitEvent = () => { };

        protected virtual void OnApplicationQuit() {
            OnApplicationQuitEvent();
            onApplicationQuit.Invoke();
        }
    }
}