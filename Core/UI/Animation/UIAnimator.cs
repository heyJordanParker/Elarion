using System;
using Elarion.Managers;
using Elarion.Utility.PropertyTweeners.CanvasGroup;
using Elarion.Utility.PropertyTweeners.RectTransform;
using UnityEngine;

namespace Elarion.UI.Animation {
    // TODO Add a parent canvas with canvas render priority equal to the animation priority (to ensure proper anchor animations and proper rendering order)
    // TODO log a warning if two animations are of the same type
    
    [RequireComponent(typeof(UIPanel))]
    public class UIAnimator : BasicUIElement {

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

        private UIPanel _target;

        private PositionTweener _positionTweener;
        private AnchorsTweener _anchorsTweener;
        private RotationTweener _rotationTweener;
        private SizeTweener _sizeTweener;
        private AlphaTweener _alphaTweener;

        private PositionTweener PositionTweener {
            get {
                if(_positionTweener == null) {
                    _positionTweener = new PositionTweener(this) {Target = _target.Transform};
                }

                return _positionTweener;
            }
        }

        public AnchorsTweener AnchorsTweener {
            get {
                if(_anchorsTweener == null) {
                    _anchorsTweener = new AnchorsTweener(this) {Target = _target.Transform};
                }

                return _anchorsTweener;
            }
        }

        public RotationTweener RotationTweener {
            get {
                if(_rotationTweener == null) {
                    _rotationTweener = new RotationTweener(this) {Target = _target.Transform};
                }

                return _rotationTweener;
            }
        }

        public SizeTweener SizeTweener {
            get {
                if(_sizeTweener == null) {
                    _sizeTweener = new SizeTweener(this) {Target = _target.Transform};
                }

                return _sizeTweener;
            }
        }

        public AlphaTweener AlphaTweener {
            get {
                if(_alphaTweener == null) {
                    _alphaTweener = new AlphaTweener(this) {Target = _target.CanvasGroup};
                }

                return _alphaTweener;
            }
        }


        protected override void Awake() {
            base.Awake();
            _target = GetComponent<UIPanel>();
        }

        protected virtual void Update() {
            _target.InTransition = AlphaTweener.Tweening || AnchorsTweener.Tweening || PositionTweener.Tweening ||
                                   RotationTweener.Tweening || SizeTweener.Tweening;
        }

        private void OnDisable() {
            ResetToSavedProperties();
        }

        private void OnDestroy() {
            ResetToSavedProperties();
        }

        public void Play(UIAnimationType animationType, bool resetToSavedProperties = false, Action callback = null) {
            Play(GetAnimation(animationType), resetToSavedProperties, callback);
        }

        public void Play(UIAnimation animation, bool resetToSavedProperties = false, Action callback = null) {
            if(resetToSavedProperties) {
                ResetToSavedProperties();
            }

            if(animation == null) {
                if(callback != null) {
                    callback();
                }
                return;
            }
            
            animation.Animate(this, callback);
        }


        public void Stop() { }

        private UIAnimation GetAnimation(UIAnimationType type) {
            foreach(var typedAnimation in _animations) {
                if(typedAnimation.type == type) {
                    return typedAnimation.animation;
                }
            }

            return null;
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

        public void Fade(float fade, UIAnimationDirection animationDirection = UIAnimationDirection.To,
            Action callback = null, UIAnimationOptions animationOptions = null) {
            
            AlphaTweener.Tween(fade, animationDirection, callback, animationOptions);
        }

        public void SaveProperties() {
            PositionTweener.SaveProperty();
            AnchorsTweener.SaveProperty();
            RotationTweener.SaveProperty();
            SizeTweener.SaveProperty();
            AlphaTweener.SaveProperty();
        }
        
        public void ResetToSavedProperties() {
            PositionTweener.ResetProperty();
            AnchorsTweener.ResetProperty();
            RotationTweener.ResetProperty();
            SizeTweener.ResetProperty();
            AlphaTweener.ResetProperty();
        }

        protected static UIManager UIManager {
            get { return Singleton.Get<UIManager>(); }
        }
    }
}