using System;
using System.Collections;
using Elarion.Common.Coroutines;
using Elarion.Common.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    /// <summary>
    /// A button that supports taps, long taps, and double taps.
    /// </summary>
    public class UIExtendedButton : Selectable, ISubmitHandler, IBeginDragHandler {
        protected enum TapType {
            Single,
            Long,
            Double
        }
        
        [SerializeField]
        private UnityEvent _onTap = new UnityEvent();

        [SerializeField]
        private UnityEvent _onLongTap = new UnityEvent();

        [SerializeField]
        private UnityEvent _onDoubleTap = new UnityEvent();

        /// <summary>
        /// Whether or not a tap event has already occured in this interaction.
        /// </summary>
        private bool _tapped;
        private bool _dragged;
        private bool _pointerDown;

        private float _lastTapTime;

        private ECoroutine _longTapCheck;

        public UnityEvent OnTap {
            get => _onTap;
            set => _onTap = value;
        }

        public UnityEvent OnLongTap {
            get => _onLongTap;
            set => _onLongTap = value;
        }

        public UnityEvent OnDoubleTap {
            get => _onDoubleTap;
            set => _onDoubleTap = value;
        }

        public override void OnPointerDown(PointerEventData eventData) {
            if(eventData.button != PointerEventData.InputButton.Left || Input.touchCount > 1)
                return;
            
            if(IsInteractable() && (navigation.mode != Navigation.Mode.None && EventSystem.current != null))
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);

            base.OnPointerDown(eventData);

            _pointerDown = true;
            _tapped = false;
            _dragged = false;

            if(_longTapCheck != null && _longTapCheck.Running) {
                _longTapCheck.Stop();
            }

            _longTapCheck = this.CreateCoroutine(LongTapCheck());
        }

        public override void OnPointerUp(PointerEventData eventData) {
            if(eventData.button != PointerEventData.InputButton.Left || Input.touchCount > 1)
                return;

            base.OnPointerUp(eventData);

            _pointerDown = false;

            if(!_tapped && !_dragged) {
                Tap(TapType.Single);
            }
        }


        public virtual void OnBeginDrag(PointerEventData eventData) {
            if(!MayDrag(eventData)) {
                return;
            }

            _dragged = true;
        }

        public virtual void OnSubmit(BaseEventData eventData) {
            Tap(TapType.Single);

            if(!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);

            StartCoroutine(OnFinishSubmit());
        }

        private bool MayDrag(PointerEventData eventData) {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left && Input.touchCount <= 1;
        }

        private IEnumerator OnFinishSubmit() {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while(elapsedTime < fadeTime) {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        private void Tap(TapType type) {
            if(_tapped || !IsActive() || !IsInteractable())
                return;

            if(type == TapType.Single &&
               Time.time - _lastTapTime <= UIManager.DoubleTapTimeout) {
                type = TapType.Double;
            }

            _tapped = true;

            switch(type) {
                case TapType.Single:
                    OnTap.Invoke();
                    _lastTapTime = Time.time;
                    break;
                case TapType.Long:
                    OnLongTap.Invoke();
                    break;
                case TapType.Double:
                    OnDoubleTap.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private IEnumerator LongTapCheck() {
            var startingTime = Time.time;

            while(_pointerDown && !_dragged && Input.touchCount <= 1) {
                if(Time.time - startingTime >= UIManager.LongTapTimeout) {
                    Tap(TapType.Long);
                    break;
                }

                yield return EWait.ForEndOfFrame;
            }
        }

        private static UIManager UIManager => UIManager.Instance;
    }
}