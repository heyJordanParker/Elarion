using Elarion.Extensions;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    public class UIForm : UIPopup { }

    public class UIPopup : UIPanel {
        public override void OnSubmit(BaseEventData eventData) {
            base.OnSubmit(eventData);
            
            // TODO click submit button
            
            // TODO click cancel button
        }
    }

    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIPanel : UIComponent {
        // TODO UIForm inheritor - add error checking submitting and so on builtin (submit with enter/submit input (in unity))
        // TODO UIDialog inheritor - dynamic amount of (getcomponent; onchildren changed), extensible; dialog skins?

        [SerializeField]
        private GameObject _firstFocused;

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

        protected override void AfterOpen() {
            base.AfterOpen();

            if(_firstFocused == null) {
                return;
            }

            var component = _firstFocused.GetComponent<UIComponent>();

            if(component) {
                component.Focus();
            }
            
            var selectable = _firstFocused.gameObject.GetFirstSelectableChild();

            if(selectable) {
                UIRoot.Focus(selectable);
            }
        }

        protected override void OnValidate() {
            base.OnValidate();

            if(_firstFocused != null) {
                if(!_firstFocused.transform.IsChildOf(transform) || _firstFocused.gameObject == gameObject) {
                    _firstFocused = null;
                }
            }

            if(_firstFocused == null) {
                foreach(var component in gameObject.GetComponentsInChildren<UIComponent>()) {
                    if(component != this) {
                        _firstFocused = component.gameObject;
                        break;
                    }
                }
            }
        }
    }
}