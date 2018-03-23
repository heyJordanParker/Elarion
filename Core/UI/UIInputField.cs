using System;
using Elarion.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    public class UIInputField : InputField {
        
        // TODO input validator - a few builtin options and a custom regex option (as enum) and a length validator (with minmax slider)
        
        public const string EmailRegex = "";
        public const string PasswordRegex = "";

        private enum InputValidation {
            None,
            Email,
            Password,
            Length,
            Regex
        }
        
        [Serializable]
        private enum SubmitAction {
            None,
            SubmitDialog,
            SubmitTarget
        }

        [SerializeField]
        private SubmitAction _onSubmit = SubmitAction.SubmitDialog;

        [SerializeField]
        [ConditionalVisibility("_onSubmit == SubmitTarget")]
        private GameObject _submitTarget = null;

        protected override void Awake() {
            base.Awake();

            if(_onSubmit == SubmitAction.SubmitDialog) {
                var parentComponent = GetComponentInParent<UIDialog>();
                
                _submitTarget = parentComponent ? parentComponent.gameObject : null;
            }
        }

        public override void OnSubmit(BaseEventData eventData) {
            Debug.Log("SUbmit called");
            base.OnSubmit(eventData);

            if(!IsActive() || !IsInteractable() || _onSubmit == SubmitAction.None || !_submitTarget) {
                return;
            }

            ExecuteEvents.Execute(_submitTarget, null, ExecuteEvents.submitHandler);

        }
    }
}