using UnityEngine;
using UnityEngine.Events;

namespace Elarion.DataBinding.Events.Triggers {
    public abstract class EventTrigger : MonoBehaviour {

        [SerializeField]
        protected UnityEvent triggeredEvent;
    }
}