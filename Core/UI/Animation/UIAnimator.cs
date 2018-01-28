using System;
using Elarion.Attributes;
using Elarion.Managers;
using Elarion.Utility;
using Elarion.Utility.PropertyTweeners.CanvasGroup;
using Elarion.Utility.PropertyTweeners.RectTransform;
using UnityEngine;

namespace Elarion.UI.Animation {
    // TODO decouple this from the UIElement
    
    // TODO log a warning if two animations are of the same type
    
    [RequireComponent(typeof(UIElement))]
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

        private UIElement _target;

        [SerializeField, ReadOnly]
        private UIAnimation _currentAnimation = null;
        
        private Canvas _canvas;
        private RectTransform _canvasTransform;

        private PositionTweener _positionTweener;
        private AnchorsTweener _anchorsTweener;
        private RotationTweener _rotationTweener;
        private SizeTweener _sizeTweener;
        private AlphaTweener _alphaTweener;
        
        private RectTransform _targetParent;

        internal PositionTweener PositionTweener {
            get {
                if(_positionTweener == null) {
                    _positionTweener = new PositionTweener(this) {Target = _target.Transform};
                }

                return _positionTweener;
            }
        }

        internal AnchorsTweener AnchorsTweener {
            get {
                if(_anchorsTweener == null) {
                    _anchorsTweener = new AnchorsTweener(this) {Target = _target.Transform};
                }

                return _anchorsTweener;
            }
        }

        internal RotationTweener RotationTweener {
            get {
                if(_rotationTweener == null) {
                    _rotationTweener = new RotationTweener(this) {Target = _target.Transform};
                }

                return _rotationTweener;
            }
        }

        internal SizeTweener SizeTweener {
            get {
                if(_sizeTweener == null) {
                    _sizeTweener = new SizeTweener(this) {Target = _target.Transform};
                }

                return _sizeTweener;
            }
        }

        
        internal AlphaTweener AlphaTweener {
            get {
                if(_alphaTweener == null) {
                    _alphaTweener = new AlphaTweener(this) {Target = _target.CanvasGroup};
                }

                return _alphaTweener;
            }
        }

        public bool Animating {
            get {
                return AlphaTweener.Tweening || AnchorsTweener.Tweening || PositionTweener.Tweening ||
                       RotationTweener.Tweening || SizeTweener.Tweening;
            }
        }


        protected override void Awake() {
            base.Awake();
            _target = GetComponent<UIElement>();
            _currentAnimation = null;
        }

        protected virtual void OnAnimationStart(UIAnimation animation) {
            
            // TODO concurrent animations; hover & click at the same time
            // track all animations; some animations stop other animations (e.g. close stops open)
            if(_currentAnimation != null) {
                _currentAnimation.Stop(this);
            }
            
            if(_canvas == null) {
                _canvas = UIHelper.CreateAnimatorCanvas(out _canvasTransform, "Animator Canvas", transform);
            }

            _currentAnimation = animation;
            
            if(_target.Transform.parent != _canvasTransform)
                _targetParent = _target.Transform.parent.GetComponent<RectTransform>();

            _canvasTransform.SetParent(_targetParent, false);
            _canvasTransform.pivot = _target.Transform.pivot;

            _canvasTransform.anchorMin = new Vector2(AnchorsTweener.SavedValue.x, AnchorsTweener.SavedValue.y);
            _canvasTransform.anchorMax = new Vector2(AnchorsTweener.SavedValue.z, AnchorsTweener.SavedValue.w);
            
            _canvasTransform.anchoredPosition = PositionTweener.SavedValue;
            _canvasTransform.sizeDelta = SizeTweener.SavedValue;
            
            if(animation.overrideParentAnchors) {
                var cachedPosition = _canvasTransform.position;
                var cachedWidth = _canvasTransform.rect.width;
                var cachedHeight = _canvasTransform.rect.height;
                
                _canvasTransform.anchorMin = animation.overrideParentAnchorMin;
                _canvasTransform.anchorMax = animation.overrideParentAnchorMax;
                
                _canvasTransform.position = cachedPosition;
                _canvasTransform.sizeDelta = new Vector2(_canvasTransform.rect.width + (cachedWidth - _canvasTransform.rect.width),
                    _canvasTransform.rect.height + (cachedHeight - _canvasTransform.rect.height));
            }

            _canvas.sortingOrder = animation.animationPriority;

            _target.Transform.SetParent(_canvasTransform, true);

        }

        
        // TODO tweeners should have their callbacks synchronized
        // Currently they're calling the callback function multiple times
        // Also, if one tweener is stopped - others continue running
        protected virtual void OnAnimationEnd(UIAnimation animation) {
            _target.Transform.SetParent(_targetParent, true);

            _currentAnimation = null;
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
            
            OnAnimationStart(animation);

            Action animationCallback;
            
            if(callback == null) {
                animationCallback = () => {
                    OnAnimationEnd(animation);
                };
            } else {
                animationCallback = () => {
                    OnAnimationEnd(animation);
                    callback();
                };
            }

            animation.Animate(this, animationCallback);
        }

        public void Stop(bool reset = false) {
            PositionTweener.StopTween(reset);
            AnchorsTweener.StopTween(reset);
            RotationTweener.StopTween(reset);
            SizeTweener.StopTween(reset);
            AlphaTweener.StopTween(reset);
        }

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