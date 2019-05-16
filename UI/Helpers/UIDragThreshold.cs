using Elarion.Common.Attributes;
using Elarion.Common.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.UI.Helpers {
    [UIComponentHelper]
    [RequireComponent(typeof(UIDraggable))]
    public class UIDragThreshold : BaseUIBehaviour {

        [ConditionalVisibility("DragX")]
        public Vector2 xThreshold;
        
        [ConditionalVisibility("DragY")]
        public Vector2 yThreshold;

        // bool unity event; already inside parameter
        public UnityEvent onEnterThreshold;
        public UnityEvent onExitThreshold;
        
        [GetComponent, SerializeField]
        private UIDraggable _draggable;

        private bool _startedInThreshold;
        private bool _inThreshold;

        private bool DragX => _draggable && _draggable.DragX;
        private bool DragY => _draggable && _draggable.DragY;

        private RectTransform Target => _draggable.Target;

        protected override void Start() {
            base.Start();
            _draggable.OnStartDragEvent.AddListener(OnStartDrag);
            _draggable.OnEndDragEvent.AddListener(OnEndDrag);
            _inThreshold = !IsOutsideThreshold();
        }

        private void OnStartDrag() {
            _startedInThreshold = !IsOutsideThreshold();
        }

        private void OnEndDrag() {
            if(_startedInThreshold && IsOutsideThreshold()) {
                ExitThreshold();
                return;
            }

            if(!IsOutsideThreshold()) {
                EnterThreshold();
            }
        }

        private bool IsOutsideThreshold() {
            return DragX && !Target.anchoredPosition.x.InRange(xThreshold) ||
                   DragY && !Target.anchoredPosition.y.InRange(yThreshold);
        }

        private void EnterThreshold() {
            if(_inThreshold) {
                return; // already in
            }

            _inThreshold = true;
            onEnterThreshold.Invoke();
        }

        private void ExitThreshold() {
            if(!_inThreshold) {
                return; // already out
            }

            _inThreshold = false;
            onExitThreshold.Invoke();
        }
    }
}