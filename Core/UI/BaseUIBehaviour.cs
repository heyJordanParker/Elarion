using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    [RequireComponent(typeof(RectTransform))]
    public abstract class BaseUIBehaviour : UIBehaviour {
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