using System.Collections;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(UIComponent))]
    public class UIEffect : MonoBehaviour {
        // TODO this a helper component that gracefully shows/hides an effect when the main element is opened/closed
        
        // TODO no trigger; use paired with the open conditions helper to achieve the same result
        
        // TODO hook with the UIComponent's state changed event (& activate/ deactivate accordingly)

        [Header("Effect Configuration")]
        public UIEffectType type = UIEffectType.Overlay;

        [ConditionalVisibility("type == UIEffectType.Overlay")]
        public Color overlayColor = new Color(1, 1, 1, 0.5f);

        [ConditionalVisibility("type == UIEffectType.Overlay")]
        public Sprite overlayImage;
        
        [ConditionalVisibility("type == UIEffectType.Blur")]
        public int blurIntensity = 10;

        [ConditionalVisibility("type == UIEffectType.Shadow")]
        public Color shadowColor = Color.black;

        // Fade In
        [Tooltip("How long would the effect fade in view")]
        public UIAnimationDuration fadeInDuration = UIAnimationDuration.Normal;
        
        [ConditionalVisibility("fadeInDuration == UIAnimationDuration.Custom")]
        public float customFadeInDuration = 0.5f;
        
        // Fade Out
        [Tooltip("How long would the effect fade out of view")]
        public UIAnimationDuration fadeOutDuration = UIAnimationDuration.Normal;
        
        [ConditionalVisibility("fadeOutDuration == UIAnimationDuration.Custom")]
        public float customFadeOutDuration = 0.5f;

        private ECoroutine _startCoroutine;
        private ECoroutine _stopCoroutine;

        private Image ColorOverlay { get; set; }

        public Image Shadow { get; set; }

        private Image Blur { get; set; }
        
        public bool Active { get; private set; }

        public float FadeInDuration {
            get {
                if(fadeInDuration == UIAnimationDuration.Fastest)
                    // override the fastest duration; some fades make sense to be instantenious
                    return 0;
                if(fadeInDuration == UIAnimationDuration.Custom)
                    return customFadeInDuration;
                return (float) fadeInDuration / 300; // 5x faster than regular transitions
            }
        }
        
        public float FadeOutDuration {
            get {
                if(fadeOutDuration == UIAnimationDuration.Fastest)
                    // override the fastest duration; some fades make sense to be instantenious
                    return 0;
                if(fadeOutDuration == UIAnimationDuration.Custom)
                    return customFadeOutDuration;
                return (float) fadeOutDuration / 300; // 5x faster than regular transitions
            }
        }

        private Image CurrentEffect {
            get {
                switch(type) {
                    case UIEffectType.Blur: {
                        if(Blur == null) {
                            Blur = UIHelper.CreateBlurImage("Blur Effect");
                            Blur.enabled = false;
                        }
                        UIHelper.ResetOverlayImage(Blur);
                        Blur.SetBlurIntensity(blurIntensity);
                        
                        return Blur;
                    }
                    case UIEffectType.Shadow: {
                        if(Shadow == null) {
                            Shadow = UIHelper.CreateShadowImage("Shadow Effect");
                            Shadow.enabled = false;
                        }
                        UIHelper.ResetShadowImage(Shadow);
                        Shadow.color = shadowColor;
                        
                        return Shadow;
                    }
                    case UIEffectType.Overlay: {
                        if(ColorOverlay == null) {
                            ColorOverlay = UIHelper.CreateOverlayImage("Color Overlay Effect");
                            ColorOverlay.enabled = false;
                        }
                        UIHelper.ResetOverlayImage(ColorOverlay);
                        ColorOverlay.color = overlayColor;
                        ColorOverlay.sprite = overlayImage;
                        
                        return ColorOverlay;
                    }
                    default:
                        goto case UIEffectType.Blur;
                }
            }
        }

        public void Start() {
            var targetComponent = GetComponent<UIComponent>();
            if(Active) {
                return;
            }
            
            if(_stopCoroutine != null && _stopCoroutine.Running) {
                _stopCoroutine.Stop();
            }

            Active = true;
            
            if(!targetComponent.gameObject.activeInHierarchy) {
                return;
            }
            
            CurrentEffect.rectTransform.SetParent(targetComponent.Transform, false);
            CurrentEffect.rectTransform.SetAsLastSibling();
            CurrentEffect.enabled = true;

            _startCoroutine = targetComponent.CreateCoroutine(GradualTransition(FadeInDuration, false));
        }

        public void Stop(UIComponent targetComponent) {
            if(!Active) {
                return;
            }
            
            if(_startCoroutine != null && _startCoroutine.Running) {
                _startCoroutine.Stop();
            }
            
            Active = false;

            if(!targetComponent.gameObject.activeInHierarchy) {
                return;
            }

            CurrentEffect.rectTransform.SetParent(targetComponent.Transform, false);

            _stopCoroutine = targetComponent.CreateCoroutine(GradualTransition(FadeOutDuration, true));
        }

        private IEnumerator GradualTransition(float duration, bool reverse) {
            if(duration > 0) {
                var transitionProgress = 0.0f;

                while(transitionProgress <= 1) {
                    var visibility = reverse ? 1 - transitionProgress : transitionProgress;

                    UpdateEffect(visibility);

                    transitionProgress += Time.deltaTime / duration;
                    yield return null;
                }
            }

            // TODO use coroutine callback
            if(reverse) {
                CurrentEffect.enabled = false;
            } else {
                UpdateEffect(1);
            }
        }

        private void UpdateEffect(float visibility) {
            switch(type) {
                case UIEffectType.Overlay:
                    var color = ColorOverlay.color;

                    color.a = Easing.Ease(0, overlayColor.a, visibility,
                        Ease.Linear);

                    ColorOverlay.color = color;
                    break;
                case UIEffectType.Blur:
                    Blur.SetBlurIntensity(Easing.Ease(0, blurIntensity, visibility,
                        Ease.Linear));
                    break;
                case UIEffectType.Shadow:
                    color = Shadow.color;

                    color.a = Easing.Ease(0, shadowColor.a, visibility,
                        Ease.Linear);

                    Shadow.color = color;
                    break;
                default:
                    goto case UIEffectType.Blur;
            }
        }
    }
}