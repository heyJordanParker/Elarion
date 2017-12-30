using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    [RequireComponent(typeof(EventTrigger))]
    public class ResizeHandle : BasicUIElement {

        public ResizeDirection resizeDirection;

        private EventTrigger _eventTrigger;


        public EventTrigger EventTrigger {
            get {
                if(_eventTrigger == null) {
                    _eventTrigger = GetComponent<EventTrigger>();
                }
                return _eventTrigger;
            }
            private set { _eventTrigger = value; }
        }

        protected override void Awake() {
            base.Awake();
            EventTrigger = GetComponent<EventTrigger>();
        }
    }
}