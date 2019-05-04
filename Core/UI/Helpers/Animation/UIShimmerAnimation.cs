using System.Collections;
using Elarion.Attributes;
using Elarion.Coroutines;
using Elarion.Extensions;
using Elarion.Utility.PropertyTweeners.RectTransform;
using UnityEngine;

namespace Elarion.UI.Helpers.Animation {
    [RequireComponent(typeof(UIAnimator))]
    public class UIShimmerAnimation : ExtendedBehaviour {

        [Header("Shimmer Config")]
        public Ease ease = Ease.Linear;
        public float delay = 0;
        public float duration = 1;
        public bool loop = true;

        [Header("Components")]
        public RectTransform graphic;
        public RectTransform graphicContainer;

        [GetComponent]
        [SerializeField, HideInInspector]
        private RectTransform _transform;

        private ECoroutine _animation;
        
        public void StartAnimation() {
            _animation = new ECoroutine(Animation(), this);
            _animation.OnFinished += cancelled => {
                graphicContainer.anchoredPosition = Vector2.left * (graphic.rect.width / 2);
            };
        }

        public void StopAnimation() {
            _animation?.Stop();
        }
        
        protected virtual IEnumerator Animation() {
            do {
                var movementProgress = 0.0f;

                var halfGraphicWidth = graphic.rect.width / 2;
                var startingValue = Vector2.left * halfGraphicWidth;
                var targetValue = Vector2.right * (_transform.rect.width + halfGraphicWidth);
    
                if(delay > 0) {
                    yield return new WaitForSeconds(delay);
                }
    
                while(movementProgress <= 1) {
                    graphicContainer.anchoredPosition = startingValue.EaseTo(targetValue, movementProgress, ease);
    
                    movementProgress += Time.smoothDeltaTime / duration;
                    yield return null;
                }
    
                graphicContainer.anchoredPosition = startingValue.EaseTo(targetValue, 1, ease);
            } while(loop);

        }
    }
}