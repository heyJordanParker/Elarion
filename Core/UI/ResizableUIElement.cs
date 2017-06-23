using Elarion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    public class ResizableUIElement : BasicUIElement {
        public ResizeHandle[] resizeHandles;

        public Vector2 minimumDimmensions = new Vector2(50, 50);
        public Vector2 maximumDimmensions = new Vector2(500, 500);

        private ResizeDirection _resizeDirection;

        private RectTransform.Edge? _horizontalEdge;
        private RectTransform.Edge? _verticalEdge;

        private Vector2 _initialAnchorMin;
        private Vector2 _initialAnchorMax;
        private Vector2 _initialPivot;

        public bool Resizing { get; private set; }

        protected override void Awake() {
            base.Awake();

            for(int i = 0; i < resizeHandles.Length; ++i) {
                var resizeHandle = resizeHandles[i];

                // Unity's event system doesn't support custom parameters so the extra parameter is curried
                resizeHandle.EventTrigger.AddEventTrigger(
                    eventData => {
                        _resizeDirection = resizeHandle.resizeDirection;
                        OnBeginDrag(eventData);
                    },
                    EventTriggerType.BeginDrag);

                resizeHandle.EventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
                resizeHandle.EventTrigger.AddEventTrigger(OnEndDrag, EventTriggerType.EndDrag);
            }
        }

        protected virtual void OnBeginDrag(BaseEventData data) {
            Resizing = true;

            // Cache original anchors, the resize method changes it
            _initialAnchorMin = Transform.anchorMin;
            _initialAnchorMax = Transform.anchorMax;
            _initialPivot = Transform.pivot;

            // Update Edges
            _horizontalEdge = null;
            _verticalEdge = null;

            if(_resizeDirection.HasFlag(ResizeDirection.Left)) {
                _horizontalEdge = RectTransform.Edge.Right;
            }
            if(_resizeDirection.HasFlag(ResizeDirection.Right)) {
                _horizontalEdge = RectTransform.Edge.Left;
            }

            if(_resizeDirection.HasFlag(ResizeDirection.Up)) {
                _verticalEdge = RectTransform.Edge.Bottom;
            }
            if(_resizeDirection.HasFlag(ResizeDirection.Down)) {
                _verticalEdge = RectTransform.Edge.Top;
            }
        }

        protected virtual void OnDrag(BaseEventData data) {
            var delta = ((PointerEventData)data).delta;

            Resize(delta);
        }

        protected virtual void OnEndDrag(BaseEventData data) {
            Resizing = false;
            Transform.anchorMin = _initialAnchorMin;
            Transform.anchorMax = _initialAnchorMax;
            Transform.pivot = _initialPivot;
        }

        public void Resize(Vector2 amount) {
            if(_horizontalEdge != null) {
                if(_horizontalEdge == RectTransform.Edge.Right)
                    Transform.SetInsetAndSizeFromParentEdge((RectTransform.Edge)_horizontalEdge,
                        Screen.width - Transform.position.x - Transform.pivot.x * Transform.rect.width,
                        Mathf.Clamp(Transform.rect.width - amount.x, minimumDimmensions.x, maximumDimmensions.x));
                else
                    Transform.SetInsetAndSizeFromParentEdge((RectTransform.Edge)_horizontalEdge,
                        Transform.position.x - Transform.pivot.x * Transform.rect.width,
                        Mathf.Clamp(Transform.rect.width + amount.x, minimumDimmensions.x, maximumDimmensions.x));
            }

            if(_verticalEdge != null) {
                if(_verticalEdge == RectTransform.Edge.Top)
                    Transform.SetInsetAndSizeFromParentEdge((RectTransform.Edge)_verticalEdge,
                        Screen.height - Transform.position.y - Transform.pivot.y * Transform.rect.height,
                        Mathf.Clamp(Transform.rect.height - amount.y, minimumDimmensions.y, maximumDimmensions.y));
                else
                    Transform.SetInsetAndSizeFromParentEdge((RectTransform.Edge)_verticalEdge,
                        Transform.position.y - Transform.pivot.y * Transform.rect.height,
                        Mathf.Clamp(Transform.rect.height + amount.y, minimumDimmensions.y, maximumDimmensions.y));
            }
        }
    }
}