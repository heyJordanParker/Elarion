using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    [RequireComponent(typeof(EventTrigger))]
    public class ResizeHandle : MonoBehaviour {

        public ResizeDirection resizeDirection;

        [HideInInspector]
        public EventTrigger eventTrigger;

        private void Awake() {
            eventTrigger = GetComponent<EventTrigger>();
            GetComponentInParent<UIResizable>().AddHandle(this);
        }
    }
}