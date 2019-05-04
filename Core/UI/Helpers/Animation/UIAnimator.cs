using System;
using System.Linq;
using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.Utility;
using Elarion.Utility.PropertyTweeners.RectTransform;
using Elarion.Utility.PropertyTweeners.UIComponent;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elarion.UI.Helpers.Animation {
    // TODO decouple with UIComponent; make the Target a RectTransform (and use GameObject tweeners that fetch the components they need themselves)
    [UIComponentHelper]
    public class UIAnimator : MonoBehaviour, IAnimationController {
        [Serializable]
        public class TypedAnimation {
            public UIAnimationType type = UIAnimationType.OnOpen;
            public UIAnimation animation;
        }

        [SerializeField, HideInInspector]
        private TypedAnimation[] _animations = {
            new TypedAnimation {type = UIAnimationType.OnOpen},
            new TypedAnimation {type = UIAnimationType.OnClose},
        };

        private UIComponent _target;

        [SerializeField, ReadOnly(true)]
        private UIAnimation _currentAnimation = null;

        private Canvas _canvas;
        private RectTransform _canvasTransform;

        private PositionTweener _positionTweener;
        private AnchorsTweener _anchorsTweener;
        private RotationTweener _rotationTweener;
        private ScaleTweener _scaleTweener;
        private SizeTweener _sizeTweener;
        private AlphaTweener _alphaTweener;

        private RectTransform _targetParent;

        public UIComponent Target {
            get {
                if(_target == null) {
                    _target = gameObject.GetOrAddComponent<UIComponent>();
                }

                return _target;
            }
        }

        internal PositionTweener PositionTweener {
            get {
                if(_positionTweener == null) {
                    _positionTweener = new PositionTweener(this) {Target = Target.Transform};
                }

                return _positionTweener;
            }
        }

        internal AnchorsTweener AnchorsTweener {
            get {
                if(_anchorsTweener == null) {
                    _anchorsTweener = new AnchorsTweener(this) {Target = Target.Transform};
                }

                return _anchorsTweener;
            }
        }

        internal RotationTweener RotationTweener {
            get {
                if(_rotationTweener == null) {
                    _rotationTweener = new RotationTweener(this) {Target = Target.Transform};
                }

                return _rotationTweener;
            }
        }

        internal SizeTweener SizeTweener {
            get {
                if(_sizeTweener == null) {
                    _sizeTweener = new SizeTweener(this) {Target = Target.Transform};
                }

                return _sizeTweener;
            }
        }

        internal ScaleTweener ScaleTweener {
            get {
                if(_scaleTweener == null) {
                    _scaleTweener = new ScaleTweener(this) {Target = Target.Transform};
                }

                return _scaleTweener;
            }
        }

        internal AlphaTweener AlphaTweener {
            get {
                if(_alphaTweener == null) {
                    _alphaTweener = new AlphaTweener(this) {Target = Target};
                }

                return _alphaTweener;
            }
        }

        public bool Animating {
            get {
                return AlphaTweener.Tweening || AnchorsTweener.Tweening || PositionTweener.Tweening ||
                       RotationTweener.Tweening || SizeTweener.Tweening || ScaleTweener.Tweening;
            }
        }

        protected virtual void OnEnable() {
            if(_currentAnimation != null) {
                OnAnimationEnd(_currentAnimation);
            }
        }

        protected virtual void OnDisable() {
            if(_currentAnimation != null) {
                _currentAnimation.Stop(this);
            }

            ResetToSavedProperties();
        }

        private void OnDestroy() {
            ResetToSavedProperties();
        }

        protected virtual void OnAnimationStart(UIAnimation animation) {
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode &&
               UnityEditor.EditorApplication.isPlaying) {
                return; // prevents unity from firing errors when exiting play mode
            }
#endif

            if(_canvas == null) {
                _canvas = UIHelper.CreateAnimatorCanvas(out _canvasTransform,
                    Target.gameObject.name + " (Animator Canvas)", transform);
            }

            _currentAnimation = animation;

            if(Target.Transform.parent != _canvasTransform)
                if(Target.Transform.parent)
                    _targetParent = Target.Transform.parent.transform as RectTransform;
                else
                    _targetParent = null;

            _canvasTransform.SetParent(_targetParent, false);
            _canvasTransform.SetSiblingIndex(Target.Transform.GetSiblingIndex());
            _canvasTransform.pivot = Target.Transform.pivot;

            _canvasTransform.anchorMin = new Vector2(AnchorsTweener.SavedValue.x, AnchorsTweener.SavedValue.y);
            _canvasTransform.anchorMax = new Vector2(AnchorsTweener.SavedValue.z, AnchorsTweener.SavedValue.w);

            _canvasTransform.anchoredPosition = Vector2.zero; // PositionTweener.SavedValue;
            _canvasTransform.sizeDelta = SizeTweener.SavedValue;
            _canvasTransform.localScale = ScaleTweener.SavedValue;

            if(animation.overrideParentAnchors) {
                var cachedPosition = _canvasTransform.position;
                var cachedWidth = _canvasTransform.rect.width;
                var cachedHeight = _canvasTransform.rect.height;

                _canvasTransform.anchorMin = animation.overrideParentAnchorMin;
                _canvasTransform.anchorMax = animation.overrideParentAnchorMax;

                _canvasTransform.position = cachedPosition;
                _canvasTransform.sizeDelta = new Vector2(
                    _canvasTransform.rect.width + (cachedWidth - _canvasTransform.rect.width),
                    _canvasTransform.rect.height + (cachedHeight - _canvasTransform.rect.height));
            }

            Target.Transform.SetParent(_canvasTransform, true);
        }

        protected virtual void OnAnimationEnd(UIAnimation animation) {
            if(!isActiveAndEnabled) {
                // can't do this if the game object is inactive - handle it in OnEnable
                return;
            }

#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode &&
               UnityEditor.EditorApplication.isPlaying) {
                return; // prevents unity from firing errors when exiting play mode
            }
#endif

            Target.Transform.SetParent(_targetParent, true);
            Target.Transform.SetSiblingIndex(_canvasTransform.GetSiblingIndex());

            _currentAnimation = null;
        }

        public void Play(UIAnimationType animationType, bool resetToSavedProperties = false, Action callback = null) {
            Play(GetAnimation(animationType), resetToSavedProperties, callback);
        }

        // Simple method for calling in Unity Events
        // ReSharper disable once RedundantOverload.Global
        public void Play(UIAnimation uiAnimation) {
            Play(uiAnimation, false, null);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public void Play(UIAnimation uiAnimation, bool resetToSavedProperties = false, Action callback = null, UIAnimationOptions animationOptions = null) {
            if(uiAnimation == null) {
                callback?.Invoke();

                return;
            }

            // TODO concurrent animations; hover & click at the same time
            // track all animations; some animations stop other animations (e.g. close stops open)
            if(_currentAnimation != null) {
                if(uiAnimation == GetAnimation(UIAnimationType.OnOpen) ||
                   uiAnimation == GetAnimation(UIAnimationType.OnClose)) {
                    _currentAnimation.Stop(this);
                } else {
                    return;
                }
            }

            if(resetToSavedProperties) {
                ResetToSavedProperties();
            }

            OnAnimationStart(uiAnimation);

            Action animationCallback;

            if(callback == null) {
                animationCallback = () => { OnAnimationEnd(uiAnimation); };
            } else {
                animationCallback = () => {
                    OnAnimationEnd(uiAnimation);
                    callback();
                };
            }

            uiAnimation.Animate(this, animationCallback, animationOptions);
        }

        public void Stop(bool reset = false) {
            PositionTweener.StopTween(reset);
            AnchorsTweener.StopTween(reset);
            RotationTweener.StopTween(reset);
            SizeTweener.StopTween(reset);
            ScaleTweener.StopTween(reset);
            AlphaTweener.StopTween(reset);
        }

        public bool HasAnimation(UIAnimationType animationType) {
            return _animations.Any(typedAnimation => typedAnimation.type == animationType);
        }

        public UIAnimation GetAnimation(UIAnimationType type) {
            return _animations.FirstOrDefault(typedAnimation => typedAnimation.type == type)?.animation;
        }

        public void Move(Vector3 position, UIAnimationDirection animationDirection = UIAnimationDirection.To,
            Action callback = null, UIAnimationOptions animationOptions = null) {
            PositionTweener.Tween(position, animationDirection, callback, animationOptions);
        }

        public void MoveAnchors(Vector2 anchorMin, Vector2 anchorMax,
            UIAnimationDirection animationDirection = UIAnimationDirection.From, Action callback = null,
            UIAnimationOptions animationOptions = null) {
            var anchors = new Vector4(anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y);

            AnchorsTweener.Tween(anchors, animationDirection, callback, animationOptions);
        }

        public void Rotate(Vector3 rotation, UIAnimationDirection animationDirection = UIAnimationDirection.From,
            Action callback = null, UIAnimationOptions animationOptions = null) {
            RotationTweener.Tween(rotation, animationDirection, callback, animationOptions);
        }

        public void Resize(Vector2 size, UIAnimationDirection animationDirection = UIAnimationDirection.RelativeTo,
            Action callback = null, UIAnimationOptions animationOptions = null) {
            SizeTweener.Tween(size, animationDirection, callback, animationOptions);
        }
        
        public void Scale(Vector3 scale, UIAnimationDirection animationDirection = UIAnimationDirection.From,
            Action callback = null, UIAnimationOptions animationOptions = null) {
            ScaleTweener.Tween(scale, animationDirection, callback, animationOptions);
        }

        public void Fade(float fade, UIAnimationDirection animationDirection = UIAnimationDirection.To,
            Action callback = null, UIAnimationOptions animationOptions = null) {
            AlphaTweener.Tween(fade, animationDirection, callback, animationOptions);
        }

        public void SaveProperties() {
            PositionTweener.SaveProperty();
            AnchorsTweener.SaveProperty();
            RotationTweener.SaveProperty();
            SizeTweener.SaveProperty();
            ScaleTweener.SaveProperty();
            AlphaTweener.SaveProperty();
        }

        public void ResetToSavedProperties() {
            PositionTweener.ResetProperty();
            AnchorsTweener.ResetProperty();
            RotationTweener.ResetProperty();
            SizeTweener.ResetProperty();
            ScaleTweener.ResetProperty();
            AlphaTweener.ResetProperty();
        }

        public void ResetToSavedPropertiesGraceful(float duration = 0.3f) {
            PositionTweener.ResetPropertyGraceful(duration);
            AnchorsTweener.ResetPropertyGraceful(duration);
            RotationTweener.ResetPropertyGraceful(duration);
            SizeTweener.ResetPropertyGraceful(duration);
            ScaleTweener.ResetPropertyGraceful(duration);
            AlphaTweener.ResetPropertyGraceful(duration);
        }
    }
}