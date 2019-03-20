using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnUpdateHandler : MonoBehaviour {
        public UnityEvent onUpdate;

        public event Action OnUpdateEvent = () => { };

        protected virtual void Update() {
            OnUpdateEvent();
            onUpdate.Invoke();
        }
    }
}