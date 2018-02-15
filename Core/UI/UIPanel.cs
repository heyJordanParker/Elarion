using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIPanel : UIComponent {
        // TODO UIForm inheritor - add error checking submitting and so on builtin (submit with enter/submit input (in unity))
        // TODO UIDialog inheritor - dynamic amount of (getcomponent; onchildren changed), extensible; dialog skins?
        
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        public override bool Disabled {
            get { return base.Disabled; }
            set {
                base.Disabled = value;
                CanvasGroup.interactable = !value;
            }
        }

        public override float Alpha {
            get { return CanvasGroup.alpha; }
            set { CanvasGroup.alpha = Mathf.Clamp01(value); }
        }

        protected override Behaviour Render {
            get {
                if(_canvas == null) {
                    _canvas = GetComponent<Canvas>();
                }
                return _canvas;
            }
        }

        protected CanvasGroup CanvasGroup {
            get {
                if(_canvasGroup == null) {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }

        public override void Focus() {
            base.Focus();
            
            var selectable = GetComponentInChildren<Selectable>();

            if(selectable) {
                UIRoot.Focus(selectable);
            }
        }
    }
}