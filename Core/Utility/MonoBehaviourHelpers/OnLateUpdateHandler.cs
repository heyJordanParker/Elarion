using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnLateUpdateHandler : MonoBehaviour {
        public UnityEvent onLateUpdate;

        public event Action OnLateUpdateEvent = () => { };

        protected virtual void LateUpdate() {
            OnLateUpdateEvent();
            onLateUpdate.Invoke();
        }
    }
}