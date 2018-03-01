using System.Linq;
using Elarion.Extensions;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    
    // TODO input validator component - a few builtin options and a custom regex option (as enum) and a length validator (with minmax slider)
    
    // TODO use the OnSubmit/OnCancel handlers to add cancelation/submitting to regular unity input fields and such - just add them on runtime and hook the appropriate events

    public class UIPopup : UIDialog {
        // basic info dialog - both submit and cancel close it; minimal inputs
    }
    
    public class UIForm : UIDialog {
        // Add those below on the first update
        // input field class - key and value (both strings for easy www calls), optional validation, etc
        // auto-focus invalid input fields
        // show errors
        
        // TODO get all inputfields and button children on awake
        // TODO add onsubmit event to all inputfields and add a submit event to the button
    }

    public class UIDialog : UIPanel {
        
        // TODO cache the object that was last focused before opening this and focus it back when closing

        // dropdown
        public bool submitOnDeselect;
        public bool cancelOnDeselect;


        // TODO add submit, cancel hanlders here

    }

    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIPanel : UIComponent {
        // TODO UIForm inheritor - add error checking submitting and so on builtin (submit with enter/submit input (in unity))
        // TODO UIDialog inheritor - dynamic amount of (getcomponent; onchildren changed), extensible; dialog skins?

        [SerializeField]
        private bool _interactable = true;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        public override float Alpha {
            get { return CanvasGroup.alpha; }
            set { CanvasGroup.alpha = Mathf.Clamp01(value); }
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
    }
}