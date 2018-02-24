using System;
using System.Linq;
using Elarion.Attributes;
using Elarion.Utility;
using Elarion.Utility.PropertyTweeners.RectTransform;
using Elarion.Utility.PropertyTweeners.UIComponent;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elarion.UI.Animation {
    // TODO add effects here; duplicate the animation editor for them but use state dropdown + effect
    
    // TODO handle effects?
    
    // TODO log a warning if two animations are of the same type (start iterating from the last animation and change/remove animations until this is satisfied)
    
    // TODO decouple with UIComponent; make the Target a RectTransform (and use GameObject tweeners that fetch the components they need themselves)
    
    [RequireComponent(typeof(UIComponent))]
    public class UIAnimator : MonoBehaviour {

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

        public UIComponent Target {
            get {
                if(_target == null) {
                    _target = GetComponent<UIComponent>();
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
                       RotationTweener.Tweening || SizeTweener.Tweening;
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
            if(_canvas == null) {
                _canvas = UIHelper.CreateAnimatorCanvas(out _canvasTransform, Target.gameObject.name + " (Animator Canvas)", transform);
            }

            _currentAnimation = animation;
            
            if(Target.Transform.parent != _canvasTransform)
                if(Target.Transform.parent)
                    _targetParent = Target.Transform.parent.GetComponent<RectTransform>();
                else
                    _targetParent = null;

            _canvasTransform.SetParent(_targetParent, false);
            _canvasTransform.SetSiblingIndex(Target.Transform.GetSiblingIndex());
            _canvasTransform.pivot = Target.Transform.pivot;

            _canvasTransform.anchorMin = new Vector2(AnchorsTweener.SavedValue.x, AnchorsTweener.SavedValue.y);
            _canvasTransform.anchorMax = new Vector2(AnchorsTweener.SavedValue.z, AnchorsTweener.SavedValue.w);
            
            _canvasTransform.anchoredPosition = PositionTweener.SavedValue;
            _canvasTransform.sizeDelta = SizeTweener.SavedValue;
            _canvasTransform.localScale = Target.Transform.localScale;
            
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

            Target.Transform.SetParent(_canvasTransform, true);

        }

        protected virtual void OnAnimationEnd(UIAnimation animation) {
            if(!gameObject.activeInHierarchy) {
                // can't do this if the game object is inactive - handle it in OnEnable
                return;
            }
            
            Target.Transform.SetParent(_targetParent, true);
            Target.Transform.SetSiblingIndex(_canvasTransform.GetSiblingIndex());

            _currentAnimation = null;
        }

        public void Play(UIAnimationType animationType, bool resetToSavedProperties = false, Action callback = null) {
            Play(GetAnimation(animationType), resetToSavedProperties, callback);
        }

        public void Play(UIAnimation animation, bool resetToSavedProperties = false, Action callback = null) {
            
            if(animation == null) {
                if(callback != null) {
                    callback();
                }
                return;
            }
            
            // TODO concurrent animations; hover & click at the same time
            // track all animations; some animations stop other animations (e.g. close stops open)
            if(_currentAnimation != null) {
                if(animation == GetAnimation(UIAnimationType.OnOpen) ||
                   animation == GetAnimation(UIAnimationType.OnClose)) {
                    _currentAnimation.Stop(this);    
                } else {
                    return;
                }
            }
            
            if(resetToSavedProperties) {
                ResetToSavedProperties();
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

        public bool HasAnimation(UIAnimationType animationType) {
            return _animations.Any(typedAnimation => typedAnimation.type == animationType);

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
        
        public void ResetToSavedPropertiesGraceful(float duration = 0.3f) {
            PositionTweener.ResetPropertyGraceful(duration);
            AnchorsTweener.ResetPropertyGraceful(duration);
            RotationTweener.ResetPropertyGraceful(duration);
            SizeTweener.ResetPropertyGraceful(duration);
            AlphaTweener.ResetPropertyGraceful(duration);
        }

#if UNITY_EDITOR

        // Editor-only helper field/logic to add a mask to the game object
        [Tooltip("Prevents child animations from overflowing.")]
        public bool maskChildAnimations = false;
        
        private void OnValidate() {
            if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
                return;
            }
            
            if(maskChildAnimations) {
                if(gameObject.GetComponent<Mask>()) {
                    return;
                }
                
                gameObject.AddComponent<Mask>();
                
                if(!gameObject.GetComponent<Graphic>()) {
                    var image = gameObject.AddComponent<Image>();
                    image.color = new Color(1, 1, 1, 0);
                }
            } else {
                if(!gameObject.GetComponent<Mask>()) {
                    return;
                }

                UnityEditor.EditorApplication.delayCall += () => {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    DestroyImmediate(gameObject.GetComponent<Mask>());
                };
                
            }
            
        }

#endif
    }
}