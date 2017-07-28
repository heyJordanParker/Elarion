using UnityEngine;

namespace Elarion.UI {
    [RequireComponent(typeof(RectTransform))]
    public abstract class BasicUIElement : MonoBehaviour {
        private RectTransform _transform;

        public RectTransform Transform {
            get {
                if(_transform == null) {
                    _transform = GetComponent<RectTransform>();
                }
                return _transform;
            }
            protected set { _transform = value; }
        }

        protected virtual void Awake() {
            Transform = GetComponent<RectTransform>();
        }

    }
}
