using System;
using System.Net.Sockets;
using Elarion.Attributes;
using Elarion.UI.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    // TODO open popup option
    public class UIButtonController : UIBehaviour, IPointerClickHandler, ISubmitHandler {
        [Serializable]
        private enum Type {
            OpenComponent,
            CloseComponent,
            ToggleComponent,
            OpenUIScene,
            Submit,
            Cancel,
        }
        
        [SerializeField]
        private Type _type = Type.OpenUIScene;

        [SerializeField, ConditionalVisibility("_type == Type.OpenComponent || Type.CloseComponent || Type.Submit || Type.Cancel || _type == Type.ToggleComponent")]
        private UIComponent _targetComponent;
        [SerializeField, ConditionalVisibility("_type == Type.OpenUIScene")]
        private UIScene _targetScene;
        
        [SerializeField, ConditionalVisibility("_type == Type.OpenComponent || Type.OpenUIScene")]
        private UIAnimation _openAnimationOverride;
        [SerializeField, ConditionalVisibility("_type == Type.CloseComponent || Type.OpenUIScene")]
        private UIAnimation _closeAnimationOverride;

        private UIComponent _parentComponent;
        
        public UIComponent ParentComponent {
            get {
                if(_parentComponent == null) {
                    _parentComponent = GetComponentInParent<UIComponent>();
                }
                return _parentComponent;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            ClickHandler(false);
        }

        public void OnSubmit(BaseEventData eventData) {
            Debug.Log("Submit");
            ClickHandler(true);
        }
        
        private void ClickHandler(bool submitEvent) {
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
                    if(_targetComponent != null && _targetComponent.Opened) {
                        goto case Type.CloseComponent;
                    }
                    goto case Type.OpenComponent;
                case Type.OpenUIScene:
                    openComponent = _targetScene;
                    closeComponent = _targetScene.UIRoot ? _targetScene.UIRoot.CurrentScene : null;
                    break;
                case Type.Cancel:
                    if(_targetComponent != null) {
                        if(!submitEvent || _targetComponent.UIRoot.FocusedComponent != _targetComponent)
                            _targetComponent.OnCancel(null);
                    }

                    break;
                case Type.Submit:
                    if(_targetComponent != null)
                        if(!submitEvent || _targetComponent.UIRoot.FocusedComponent != _targetComponent)
                            _targetComponent.OnSubmit(null);
                    break;
            }

            if(openComponent) {
                if(openComponent.Opened) {
                    openComponent.Focus(true);
                } else {
                    openComponent.Open(overrideAnimation: _openAnimationOverride);
                }
            }

            if(closeComponent) {
                closeComponent.Close(overrideAnimation: _closeAnimationOverride);
            }
        }
    }
}