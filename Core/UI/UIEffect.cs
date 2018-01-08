using System;
using System.Collections;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.Managers;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [Serializable]
    public class UIEffect {
        // TODO FadeIn & FadeOut effects (play with the panel's alpha)
        
        // Move/MoveAnchor, Scale, and Rotate effects - e.g. a note can scale down while moving
        
        // TODO show/hide something effect (show loaders & such)

        // TODO flags attribute (so that multiple states can be selected from the inspector); make sure it works with the conditional visibility
        [Header("In which UIState should the effect activate")]
        public UIState state = UIState.InTransition;

        [Header("Effect Configuration")]
        public UIEffectType type = UIEffectType.Overlay;

        [ConditionalVisibility("type == UIEffectType.Overlay")]
        public Color overlayColor = new Color(1, 1, 1, 0.5f);

        [ConditionalVisibility("type == UIEffectType.Overlay")]
        public Sprite overlayImage;
        
        // overlay prefab?

        [ConditionalVisibility("type == UIEffectType.Blur")]
        public int blurIntensity = 10;

        [ConditionalVisibility("type == UIEffectType.Shadow")]
        public Color shadowColor = Color.black;

        [Tooltip("How long would the effect fade in and fade out of view")]
        public UIAnimationDuration fadeDuration = UIAnimationDuration.Normal;

        [ConditionalVisibility("fadeDuration == UIEffectFade.Custom")]
        public float customFadeDuration = 0.5f;

        private ECoroutine _startCoroutine;
        private ECoroutine _stopCoroutine;

        private Image ColorOverlay { get; set; }

        public Image Shadow { get; set; }

        private Image Blur { get; set; }

        public float FadeDuration {
            get {
                if(fadeDuration == UIAnimationDuration.Instant)
                    // override the instant duration; some fades make sense to be instantenious
                    return 0;
                if(fadeDuration == UIAnimationDuration.Custom)
                    return customFadeDuration;
                return (int) fadeDuration / 100f;
            }
        }

        private Image CurrentEffect {
            get {
                switch(type) {
                    case UIEffectType.Blur: {
                        if(Blur == null) {
                            Blur = UIHelper.CreateBlurImage("Blur Effect", UIManager.MainCanvas.transform);
                            Blur.enabled = false;
                        }
                        Blur.SetBlurIntensity(blurIntensity);
                        
                        return Blur;
                    }
                    case UIEffectType.Shadow: {
                        if(Shadow == null) {
                            Shadow = UIHelper.CreateShadowImage("Shadow Effect", UIManager.MainCanvas.transform);
                            Shadow.enabled = false;
                        }
                        Shadow.color = shadowColor;
                        
                        return Shadow;
                    }
                    case UIEffectType.Overlay: {
                        if(ColorOverlay == null) {
                            ColorOverlay = UIHelper.CreateOverlayImage("Color Overlay Effect",
                                UIManager.MainCanvas.transform);
                            ColorOverlay.enabled = false;
                        }
                        ColorOverlay.color = overlayColor;
                        ColorOverlay.sprite = overlayImage;
                        
                        return ColorOverlay;
                    }
                    default:
                        goto case UIEffectType.Blur;
                }
            }
        }

        public void Start(UIPanel panel) {
            if(_stopCoroutine != null && _stopCoroutine.Running) {
                _stopCoroutine.Stop();
            }
            
            CurrentEffect.rectTransform.SetParent(panel.AnimationTarget, false);
            CurrentEffect.enabled = true;

            _startCoroutine = panel.CreateCoroutine(GradualTransition(FadeDuration, false));
        }

        public void Stop(UIPanel panel) {
            if(_startCoroutine != null && _startCoroutine.Running) {
                _startCoroutine.Stop();
            }

            CurrentEffect.rectTransform.SetParent(panel.AnimationTarget, false);

            _stopCoroutine = panel.CreateCoroutine(GradualTransition(FadeDuration, true));
        }

        private IEnumerator GradualTransition(float duration, bool reverse) {
            var transitionProgress = 0.0f;

            while(transitionProgress <= 1) {
                var visibility = reverse ? 1 - transitionProgress : transitionProgress;

                UpdateEffect(visibility);

                transitionProgress += Time.deltaTime / duration;
                yield return null;
            }

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

        private static UIManager UIManager {
            get { return Singleton.Get<UIManager>(); }
        }
    }
}