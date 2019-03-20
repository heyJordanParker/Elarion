using System.Collections;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI.Utils {
    [RequireComponent(typeof(RectTransform))]
    public class UITapIndicator : PooledObject {

        public Graphic graphic;
        
        [Header("Pointer Down")]
        public float pointerDownAnimationDuration = 2f;
        public Ease pointerDownAnimationEase = Ease.InOutQuad;

        [Header("Pointer Down")]
        public float pointerUpAnimationDuration = 1f;
        public Ease pointerUpAnimationEase = Ease.InQuad;

        [Header("Animation Size")]
        public Vector2 maxSize = new Vector2(10, 10);
        
        [SerializeField, GetComponent]
        protected new RectTransform transform;

        [SerializeField, HideInInspector]
        protected Transform graphicTransform;

        [SerializeField, HideInInspector]
        protected Color graphicOriginalColor;

        protected override void OnInitialize() {
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.sizeDelta = Vector2.zero;
            transform.anchoredPosition = Vector2.zero;
            graphic.color = graphicOriginalColor;
            graphicTransform.localScale = Vector3.one;
            
            base.OnInitialize();
        }

        public IEnumerator OnPointerDownAnimation() {
            graphicTransform.localScale = Vector3.one;
            
            var size = graphicTransform.localScale;

            var time = 0f;
            
            while(time <= pointerDownAnimationDuration) {
                size = size.EaseTo(maxSize, time / pointerDownAnimationDuration, pointerDownAnimationEase);

                graphicTransform.localScale = size;

                time += Time.deltaTime;
                yield return null;
            }
        }

        public IEnumerator OnPointerUpAnimation() {
            var alpha = graphic.color.a;

            var time = 0f;
            
            while(time <= pointerUpAnimationDuration) {
                var progress = time / pointerUpAnimationDuration;
                
                alpha = alpha.EaseTo(0, progress, pointerUpAnimationEase);

                var color = graphic.color;
                color.a = alpha;
                graphic.color = color;

                time += Time.deltaTime;
                yield return null;
            }
        }


        protected override void OnValidate() {
            base.OnValidate();

            if(!graphic) {
                return;
            }
            
            graphicOriginalColor = graphic.color;
                
            if(!graphicTransform) {
                graphicTransform = graphic.transform;
            }
        }
    }
}