using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI.Helpers {
    [RequireComponent(typeof(EventTrigger))]
    public class UIResizeHandle : MonoBehaviour {

        public ResizeDirection resizeDirection;

        [HideInInspector]
        public EventTrigger eventTrigger;

        private void Awake() {
            eventTrigger = GetComponent<EventTrigger>();
            GetComponentInParent<UIResizable>().AddHandle(this);
        }
    }
}