using Elarion.Extensions;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    public class UIForm : UIPopup { }

    public class UIPopup : UIPanel {
        protected override void OnSubmitInternal(BaseEventData eventData) {
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

        [SerializeField]
        private bool _interactable = true;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        public override float Alpha {
            get { return CanvasGroup.alpha; }
            set { CanvasGroup.alpha = Mathf.Clamp01(value); }
        }

        public override GameObject FirstFocused {
            get { return _firstFocused; }
            set { _firstFocused = value; }
        }

        protected override bool InteractableSelf {
            get { return _interactable; }
            set { _interactable = value; }
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

        protected override bool UpdateState() {
            if(!base.UpdateState()) {
                return false;
            }

            CanvasGroup.interactable = !Disabled;

            CanvasGroup.blocksRaycasts = Interactable;
            return true;
        }

        protected override void OnValidate() {
            base.OnValidate();

            if(_firstFocused != null) {
                if(!_firstFocused.transform.IsChildOf(transform)) {
                    _firstFocused = null;
                }
            }

            if(_firstFocused != null) {
                return;
            }
            
            foreach(var component in gameObject.GetComponentsInChildren<UIComponent>()) {
                if(component == this) {
                    continue;
                }
                
                _firstFocused = component.gameObject;
                return;
            }

            if(_firstFocused != null) {
                return;
            }
            
            var selectable = GetComponentInChildren<Selectable>();
            if(selectable != null) {
                _firstFocused = selectable.gameObject;
            }
        }
    }
}