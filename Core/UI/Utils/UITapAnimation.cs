using System.Collections;
using Elarion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI.Utils {
    // TODO dynamically instantiate tap animation prefab when I have pooling available
    
    [RequireComponent(typeof(Mask))]
    public class UITapAnimation : BaseUIBehaviour, IPointerDownHandler, IPointerUpHandler {
        
        public Graphic tapAnimation;
        
        [Header("Pointer Down")]
        public float pointerDownAnimationDuration = 2f;
        public Ease pointerDownAnimationEase = Ease.InOutQuad;

        [Header("Pointer Down")]
        public float pointerUpAnimationDuration = 1f;
        public Ease pointerUpAnimationEase = Ease.InQuad;

        [Header("Animation Size")]
        public Vector2 maxSize = new Vector2(10, 10);

        private ECoroutine _pointerDownAnimation;
        private ECoroutine _pointerUpAnimation;

        private Color _tapAnimationOriginalColor;
        private Selectable _selectableComponent;

        public Transform TapAnimationTransform => tapAnimation.transform;

        protected override void Awake() {
            base.Awake();
            TapAnimationTransform.SetActive(false);
            _tapAnimationOriginalColor = tapAnimation.color;
            _selectableComponent = GetComponent<Selectable>();
        }

        public void OnPointerDown(PointerEventData eventData) {
            if(_selectableComponent && !_selectableComponent.interactable) {
                return;
            }
            
            if(eventData.button != PointerEventData.InputButton.Left) {
                return;
            }

            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position,
                null,
                out var localPosition)) {
                return;
            }

            if(_pointerUpAnimation != null && _pointerUpAnimation.Running) {
                _pointerUpAnimation.Stop();
                _pointerUpAnimation = null;
            }
            
            TapAnimationTransform.localPosition = localPosition;
            tapAnimation.color = _tapAnimationOriginalColor;

            _pointerDownAnimation = this.CreateCoroutine(OnPointerDownAnimation());
        }

        private IEnumerator OnPointerDownAnimation() {
            TapAnimationTransform.SetActive(true);

            TapAnimationTransform.localScale = Vector3.one;
            
            var size = TapAnimationTransform.localScale;

            var time = 0f;
            
            while(time <= pointerDownAnimationDuration) {
                size = size.EaseTo(maxSize, time / pointerDownAnimationDuration, pointerDownAnimationEase);

                TapAnimationTransform.localScale = size;

                time += Time.deltaTime;
                yield return null;
            }
        }

        public void OnPointerUp(PointerEventData eventData) {
            if(_pointerDownAnimation != null && _pointerDownAnimation.Running) {
                _pointerDownAnimation.Stop();
                _pointerDownAnimation = null;
            }

            _pointerUpAnimation = this.CreateCoroutine(OnPointerUpAnimation());
            
            _pointerUpAnimation.OnFinished += stopped =>             TapAnimationTransform.SetActive(false);
        }
        
        private IEnumerator OnPointerUpAnimation() {
            var alpha = tapAnimation.color.a;

            var time = 0f;
            
            while(time <= pointerUpAnimationDuration) {
                var progress = time / pointerUpAnimationDuration;
                
                alpha = alpha.EaseTo(0, progress, pointerUpAnimationEase);

                var color = tapAnimation.color;
                color.a = alpha;
                tapAnimation.color = color;

                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}