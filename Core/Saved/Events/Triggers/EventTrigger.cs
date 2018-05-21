using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Saved.Events.Triggers {
    public abstract class EventTrigger : MonoBehaviour {

        [SerializeField]
        protected UnityEvent triggeredEvent;
    }
}