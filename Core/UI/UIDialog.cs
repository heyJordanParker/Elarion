using System;
using System.Linq;
using Elarion.Extensions;
using Elarion.UI.Helpers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Elarion.UI {
    
    public class UIDialog : UIPanel, ISubmitHandler, ICancelHandler {

        [Serializable]
        protected enum DeselectAction {
            None,
            Submit,
            Cancel
        }
        
        // TODO cache the object that was last focused before opening this and focus it back when closing
        
        // dialog submit button; is it necessary?

        [SerializeField]
        protected Button submitButton;

        [SerializeField]
        protected DeselectAction deselectAction;
        
        // close/submit dialogs when the scene changes/gets deselected
        
        // TODO Timeout component (as in graphic settings) - executes an action when the timer expires (also updates a UI element)
        
        // TODO scene changed event

        protected override void Awake() {
            base.Awake();
            
            switch(deselectAction) {
                case DeselectAction.None:
                    break;
                case DeselectAction.Submit:
                    Blurred += Submit;
                    break;
                case DeselectAction.Cancel:
                    Blurred += Cancel;
                    break;
                default:
                    goto case DeselectAction.None;
            }
        }

        public void Test() {
            Debug.Log("Test");
        }

        protected virtual void Submit() {
            if(!IsOpened) {
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