using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIElement : UIComponent {
        
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        
        public override float Alpha {
            get { return CanvasGroup.alpha; }
            set { CanvasGroup.alpha = Mathf.Clamp01(value); }
        }

        public override Behaviour Renderer {
            get {
                if(_canvas == null) {
                    _canvas = GetComponent<Canvas>();
                }

                return _canvas;
            }
        }
        
        public CanvasGroup CanvasGroup {
            get {
                if(_canvasGroup == null) {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }

                return _canvasGroup;
            }
        }
        
        protected override void OnStateChanged(States currentState, States previousState) {
            base.OnStateChanged(currentState, previousState);
            // disable disabled child's interaction to simplify navigation events
            CanvasGroup.interactable = IsRendering && !IsDisabled;
        }
    }
}