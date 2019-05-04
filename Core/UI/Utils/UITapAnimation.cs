using System.Collections;
using Elarion.Attributes;
using Elarion.Coroutines;
using Elarion.Extensions;
using Elarion.Pooling;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI.Utils {
    [RequireComponent(typeof(Selectable))]
    public class UITapAnimation : BaseUIBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler {

        public ObjectPool tapIndicatorPool;

        public Camera overrideCamera = null;

        private ECoroutine _pointerDownAnimation;
        private ECoroutine _pointerUpAnimation;

        [SerializeField, GetComponent]
        private Selectable _selectableComponent;
        
        private UITapIndicator _tapIndicator;

        public void OnPointerDown(PointerEventData eventData) {
            if(_tapIndicator != null) {
                return; // can't have two
            }
            
            if(_selectableComponent && !_selectableComponent.interactable) {
                return;
            }
            
            if(eventData.button != PointerEventData.InputButton.Left || Input.touchCount > 1) {
                return;
            }

            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position,
                overrideCamera,
                out var localPosition)) {
                return;
            }

            if(_pointerUpAnimation != null && _pointerUpAnimation.Running) {
                _pointerUpAnimation.Stop();
                _pointerUpAnimation = null;
            }

            _tapIndicator = tapIndicatorPool.Spawn(transform, true) as UITapIndicator;
            
            if(_tapIndicator == null) {
                Debug.LogError("Invalid tap indicator prefab", tapIndicatorPool);
                return;
            }
            
            _tapIndicator.graphic.transform.localPosition = localPosition;
            
            _pointerDownAnimation = this.CreateCoroutine(_tapIndicator.OnPointerDownAnimation());
        }

        public void OnBeginDrag(PointerEventData eventData) {
            OnPointerUp(eventData);
        }

        public void OnPointerUp(PointerEventData eventData) {
            if(!_tapIndicator || (_pointerUpAnimation != null && _pointerUpAnimation.Running)) {
                return; // can't animate if it didn't spawn for some reason
            }
            
            if(_pointerDownAnimation != null && _pointerDownAnimation.Running) {
                _pointerDownAnimation.Stop();
                _pointerDownAnimation = null;
            }

            _pointerUpAnimation = this.CreateCoroutine(_tapIndicator.OnPointerUpAnimation());
            
            _pointerUpAnimation.OnFinished += stopped => {
                tapIndicatorPool.Return(_tapIndicator);
                _tapIndicator = null;
            };
        }
    }
}