using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Workflows.Events.Triggers {
    public abstract class EventTrigger : MonoBehaviour {

        [SerializeField]
        protected UnityEvent triggeredEvent;
    }
}