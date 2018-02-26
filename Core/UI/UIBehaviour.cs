using UnityEngine;

namespace Elarion.UI {
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIBehaviour : UnityEngine.EventSystems.UIBehaviour {
        public RectTransform Transform {
            get {
                if(_transform == null) {
                    _transform = transform as RectTransform;
                }

                return _transform;
            }    
        }
        
        private RectTransform _transform;
    }
}