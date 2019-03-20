using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnDestroyHandler : MonoBehaviour {
        public UnityEvent onDestroy;

        public event Action OnDestroyEvent = () => { };

        protected virtual void OnDestroy() {
            OnDestroyEvent();
            onDestroy.Invoke();
        }
    }
}