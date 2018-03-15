using System;
using System.Linq;
using Elarion.Extensions;
using Elarion.UI.Helpers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    
    // TODO input validator component - a few builtin options and a custom regex option (as enum) and a length validator (with minmax slider)
    
    public class UIDialog : UIPanel, ISubmitHandler, ICancelHandler {

        [Serializable]
        protected enum DeselectAction {
            None,
            Submit,
            Cancel
        }
        
        // TODO cache the object that was last focused before opening this and focus it back when closing
        
        // dialog submit button?

        [SerializeField]
        protected Button submitButton;

        [SerializeField]
        protected DeselectAction deselectAction;
        
        // close/submit dialogs when the scene changes/gets deselected/timeout

        private InputField[] _inputs;

        protected override void Awake() {
            base.Awake();
            
            _inputs = GetComponentsInChildren<InputField>();
            
            // auto-submit when child input fields get submitted 
            foreach(var input in _inputs) {
                var submitHandler = input.gameObject.Component<UISubmitHandler>();
                
                submitHandler.Submit += Submit;
            }

            switch(deselectAction) {
                case DeselectAction.None:
                    break;
                case DeselectAction.Submit:
                    State.Blurred += Submit;
                    break;
                case DeselectAction.Cancel:
                    State.Blurred += Cancel;
                    break;
                default:
                    goto case DeselectAction.None;
            }
        }

//        public override void Focus(bool setSelection = false, bool autoFocus = true) {
//            base.Focus(false, autoFocus);
//        }

        public void Test() {
            Debug.Log("Test");
        }

        protected virtual void Submit() {
            if(!State.IsOpened) {
                return;
            }
            
            // TODO cache the submitted/canceled state and reset it when opened (to preven submitting multiple times) 
            Debug.Log(name + "Submitting", gameObject);
            Close();
        }

        protected virtual void Cancel() {
            Debug.Log(name + "Canceling", gameObject);
            Close();
        }

        public void OnSubmit(BaseEventData eventData) {
            Submit();
        }

        public void OnCancel(BaseEventData eventData) {
            Cancel();
        }
    }
}