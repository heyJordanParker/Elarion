using System;
using Elarion.Attributes;
using Elarion.UI.Helpers.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    public class UIButtonController : BaseUIBehaviour, IPointerClickHandler, ISubmitHandler {
        [Serializable]
        private enum Type {
            OpenComponent,
            CloseComponent,
            ToggleComponent,
            OpenUIScene,
            SendSubmitEvent,
            SendCancelEvent,
        }
        
        [SerializeField]
        private Type _type = Type.OpenUIScene;

        [SerializeField, ConditionalVisibility("_type == Type.OpenComponent || Type.CloseComponent || _type == Type.ToggleComponent")]
        private UIComponent _targetComponent;
        [SerializeField, ConditionalVisibility("_type == Type.SendSubmitEvent")]
        private GameObject _objectToSubmit;
        [SerializeField, ConditionalVisibility("_type == Type.SendCancelEvent")]
        private GameObject _objectToCancel;
        [SerializeField, ConditionalVisibility("_type == Type.OpenUIScene")]
        private UIScene _targetScene;
        
        [SerializeField, ConditionalVisibility("_type == Type.OpenComponent || Type.OpenUIScene")]
        private UIAnimation _openAnimationOverride;
        [SerializeField, ConditionalVisibility("_type == Type.CloseComponent || Type.OpenUIScene")]
        private UIAnimation _closeAnimationOverride;

        public void OnPointerClick(PointerEventData eventData) {
            if(eventData == null || eventData.button != PointerEventData.InputButton.Left) {
                return;
            }
            
            if(EventHandler(true)) {
                eventData.Use();
            }
        }

        public void OnSubmit(BaseEventData eventData) {
            if(EventHandler(false) && eventData != null) {
                eventData.Use();
            }
        }
        
        private bool EventHandler(bool clickEvent) {
            UIComponent openComponent = null;
            UIComponent closeComponent = null;

            switch(_type) {
                case Type.OpenComponent:
                    openComponent = _targetComponent;
                    break;
                case Type.CloseComponent:
                    closeComponent = _targetComponent;
                    break;
                case Type.ToggleComponent:
                    if(_targetComponent != null && _targetComponent.State.IsOpened) {
                        goto case Type.CloseComponent;
                    }
                    goto case Type.OpenComponent;
                case Type.OpenUIScene:
                    openComponent = _targetScene;
                    closeComponent = UIScene.CurrentScene;
                    break;
                case Type.SendCancelEvent:
                    if(_objectToCancel != null) {
                        ExecuteEvents.Execute(_objectToCancel, null, ExecuteEvents.cancelHandler);
                    }
                    break;
                case Type.SendSubmitEvent:
                    if(_objectToSubmit != null) {
                        ExecuteEvents.Execute(_objectToSubmit, null, ExecuteEvents.submitHandler);
                    }
                    break;
                default:
                    return false;
            }

            if(openComponent != null) {
                openComponent.Open(overrideAnimation: _openAnimationOverride);
            }

            if(closeComponent) {
                closeComponent.Close(overrideAnimation: _closeAnimationOverride);
            }

            return true;
        }
    }
}