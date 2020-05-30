using Elarion;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.UI.PropertyTweeners;
using Elarion.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI.Helpers.Animation {
    [UIComponentHelper]
    public class UIEffect : BaseUIBehaviour, IAnimationController {
        private class UIEffectTweener : PropertyTweener<float, UIEffect> {
            public UIEffectTweener(MonoBehaviour owner) : base(owner) { }

            public override float CurrentValue {
                get => Target.Visibility;
                protected set => Target.Visibility = value;
            }
            
            protected override float UpdateValue(float startingValue, float progress, Ease ease) {
                return startingValue.EaseTo(TargetValue, progress, ease);
            }

            protected override float AddValues(float value1, float value2) {
                return value1 + value2;
            }
        }

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

        [Tooltip("How long would the effect fade in view")]
        public UIAnimationDuration fadeInDuration = UIAnimationDuration.Normal;
        
        [ConditionalVisibility("fadeInDuration == UIAnimationDuration.Custom")]
        public float customFadeInDuration = 0.5f;
        
        [Tooltip("How long would the effect fade out of view")]
        public UIAnimationDuration fadeOutDuration = UIAnimationDuration.Normal;
        
        [ConditionalVisibility("fadeOutDuration == UIAnimationDuration.Custom")]
        public float customFadeOutDuration = 0.5f;

        public bool Animating {
            get { return Visibility != 0; }
        }

        private UIComponent _component;
        private float _visibility;
        private UIEffectTweener _visibilityTweener;


        private float Visibility {
            get { return _visibility; }
            set {
                _visibility = value;
                SetVisibility(value);
            }
        }

        private UIEffectTweener VisibilityTweener {
            get {
                if(_visibilityTweener == null) {
                    _visibilityTweener = new UIEffectTweener(this) { Target = this };
                }
                return _visibilityTweener;
            }
        }

        private Image ColorOverlay { get; set; }

        public Image Shadow { get; set; }

        private Image Blur { get; set; }
        
        public bool Active { get; private set; }

        public float FadeInDuration {
            get {
                if(fadeInDuration == UIAnimationDuration.Fastest)
                    return 0; // instant
                if(fadeInDuration == UIAnimationDuration.Custom)
                    return customFadeInDuration;
                return (float) fadeInDuration / 300; // faster than usual transitions
            }
        }
        
        public float FadeOutDuration {
            get {
                if(fadeOutDuration == UIAnimationDuration.Fastest)
                    return 0; // instant
                if(fadeOutDuration == UIAnimationDuration.Custom)
                    return customFadeOutDuration;
                return (float) fadeOutDuration / 300; // faster than usual transitions
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

        protected override void Awake() {
            base.Awake();
            
            _component = gameObject.GetComponent<UIComponent>();

            if(!_component) {
                _component = gameObject.AddComponent<UIPanel>();
            }
        }

        protected override void OnEnable() {
            base.OnEnable();
                
            _component.BeforeOpenEvent.AddListener(OnOpened);
            _component.AfterCloseEvent.AddListener(OnClosed);

        }

        protected override void OnDisable() {
            base.OnDisable();
            
            _component.BeforeOpenEvent.RemoveListener(OnOpened);
            _component.AfterCloseEvent.RemoveListener(OnClosed);
        }

        public void OnOpened(bool skipAnimation) {
            if(Active) {
                return;
            }

            Active = true;
            
            if(!_component.isActiveAndEnabled) {
                return;
            }
            
            CurrentEffect.rectTransform.SetParent(_component.Transform, false);
            CurrentEffect.rectTransform.SetAsLastSibling();
            CurrentEffect.enabled = true;
            
            SetVisibility(0);

            VisibilityTweener.Tween(1, UIAnimationDirection.To, animationOptions: new UIAnimationOptions(duration: FadeInDuration));
        }

        public void OnClosed() {
            if(!Active) {
                return;
            }
            
            Active = false;

            if(!_component.isActiveAndEnabled) {
                return;
            }

            CurrentEffect.rectTransform.SetParent(_component.Transform, false);

            VisibilityTweener.Tween(0, UIAnimationDirection.To, () => CurrentEffect.enabled = false, new UIAnimationOptions(duration: FadeOutDuration));
        }

        private void SetVisibility(float visibility) {
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