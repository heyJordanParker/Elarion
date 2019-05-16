using Elarion.Common.Attributes;
using UnityEngine;

namespace Elarion.UI.Helpers {
    [UIComponentHelper]
    [RequireComponent(typeof(UIComponent))]
    [RequireComponent(typeof(UIDraggable))]
    public class UICloseOnDrag : BaseUIBehaviour {

        [SerializeField]
        private float _minimumDragDelta = 5;

        private Vector3 _cachedPosition;
        
        private UIComponent _component;
        private UIDraggable _draggable;

        protected override void Awake() {
            _component = GetComponent<UIComponent>();
            _draggable = GetComponent<UIDraggable>();
        }

        protected override void OnEnable() {
            _draggable.OnEndDragEvent.AddListener(OnEndDrag);
            _component.AfterOpenEvent.AddListener(CachePosition);
        }

        protected override void OnDisable() {
            _draggable.OnEndDragEvent.RemoveListener(OnEndDrag);
            _component.AfterOpenEvent.RemoveListener(CachePosition);
        }

        private void CachePosition() {
            _cachedPosition = transform.localPosition;
        }

        private void OnEndDrag() {
            if((_cachedPosition - transform.localPosition).magnitude <= _minimumDragDelta) {
                transform.localPosition = _cachedPosition;
            } else {
                _component.Close();
            }
        }

    }
}