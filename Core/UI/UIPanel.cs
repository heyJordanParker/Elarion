using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIPanel : UIComponent {
        // TODO UIForm inheritor - add error checking submitting and so on builtin (submit with enter/submit input (in unity))
        // TODO UIDialog inheritor - dynamic amount of (getcomponent; onchildren changed), extensible; dialog skins?
        
        // TODO interactable and disabled states - one disables raycasts by disabling the GraphicRaycaster and the other sets the canvas interactable to false

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

        protected override void OnOpen() {
            base.OnOpen();
            
            var selectable = GetComponentInChildren<Selectable>();

            if(selectable) {
                
                selectable.Select();

                var input = selectable as InputField;

                if(input != null) {
                    input.ActivateInputField();
                }
            }
        }
    }
}