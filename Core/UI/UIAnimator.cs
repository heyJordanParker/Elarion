using System;
using System.Collections;
using System.Collections.Generic;
using Elarion.Extensions;
using Elarion.Managers;
using Elarion.UI.Animations;
using Elarion.Utility;
using UnityEngine;

namespace Elarion.UI {
    public class UIAnimator : BasicUIElement {
        
        [SerializeField]
        private RectTransform _target;
        
        [SerializeField]
        private UIAnimation _moveAnimation;
        [SerializeField]
        private UIAnimation _openAnimation;
        [SerializeField]
        private UIAnimation _closeAnimation;
        [SerializeField]
        private UIAnimation _blurAnimation;
        [SerializeField]
        private UIAnimation _focusAnimation;

        [SerializeField]
        private ScriptedUIAnimation[] _scriptedAnimations;

        private UIAnimation _currentAnimation;

        private bool _animating;

        private Dictionary<UIAnimationType, ScriptedUIAnimation> _animations;

        public Dictionary<UIAnimationType, ScriptedUIAnimation> Animations {
            get { return _animations; }
        }

        public bool Moving {
            get; set;
        }

        protected override void Awake() {
            base.Awake();
            _animations = new Dictionary<UIAnimationType, ScriptedUIAnimation>();
            foreach(var scriptedUIAnimation in _scriptedAnimations) {
                if(_animations.ContainsKey(scriptedUIAnimation.type)) {
                    Debug.Log("Animator contains a duplicate scripted animation. Skipping " + scriptedUIAnimation.GetType().Name, gameObject);
                    continue;
                }
                _animations.Add(scriptedUIAnimation.type, scriptedUIAnimation);
            }

            if(Target == null) {
                Target = GetComponent<RectTransform>();
            }
        }

        public void Play(UIAnimationType animationType, Action callback = null) {
            if(Moving) {
                return;
            }

            _currentAnimation = GetAnimation(animationType); 
            
            if(_currentAnimation == null) {
                return;
            }
            
            Moving = true;

            _currentAnimation.Animate(this, Target);

            // if (int) animationType > 0 - movement towards the screen, else - movement away from the screen

            // copy the transition coroutine & respective components from the UIManager
        }

        public void Stop() {
            Moving = false;
            _currentAnimation = null;
        }

        private UIAnimation GetAnimation(UIAnimationType type) {
            UIAnimation animation = null;
            switch(type) {
                case UIAnimationType.Open:
                    animation = _openAnimation;
                    break;
                case UIAnimationType.Close:
                    animation = _closeAnimation;
                    break;
                case UIAnimationType.Blur:
                    animation = _blurAnimation;
                    break;
                case UIAnimationType.Focus:
                    animation = _focusAnimation;
                    break;
                case UIAnimationType.Move:
                    animation = _moveAnimation;
                    break;
            }

            if(animation != null) {
                return animation;
            }
            // TODO add the scripted animations here & add a common interface
            return UIManager.defaultAnimation;
        }
        
        // set CurrentPosition = SavedPosition every time the element is opened (maybe with other actions too?)
        public Vector3 SavedPosition { get; protected set; }

        public Vector3 CurrentPosition {
            get { return Target.anchoredPosition; }
            protected set { Target.anchoredPosition = value; }
        }
        
        public Vector3 TargetPosition { get; protected set; }
        
        public RectTransform Target {
            get { return _target; }
            set {
                _target = value;
                SavedPosition = TargetPosition = _target.anchoredPosition;
            }
        }
        
        // TODO Add a canvas with canvas render priority equal to the animation priority
        
        // TODO create basic move, move anchor, resize, and rotate animations
        // TODO make other animations simply call those methods (and chain them)
        // UIAnimations should have fields to configure movement, size, and rotation; do they need effects or keep them here (hooked with a panel's onStateChanged event)
        private ECoroutine _moveCoroutine;

        // Maybe this below can be moved to a separate class (Interpolator, which is inherited by MoveInterpolator, MoveAnchorInterpolator, and so on)
        public void Move(Vector3 position, UIAnimationDirection animationDirection = UIAnimationDirection.To, bool savePosition = false, bool instant = false,
            Action callback = null) {

            if(_moveCoroutine != null && _moveCoroutine.Running) {
                _moveCoroutine.Stop();
                _moveCoroutine.OnFinished += b => {
                    Move(position, animationDirection, savePosition, instant, callback);
                };
                return;
            }
            
            Moving = true;
            // TODO canvas needs to be positioned unparented before the reposition below
            // TODO the reposition below uses local coordinates and bugs if it's inside the temp canvas

            if(animationDirection == UIAnimationDirection.RelativeTo || animationDirection == UIAnimationDirection.RelativeFrom) {
                position += TargetPosition;
            }
            
            if(animationDirection == UIAnimationDirection.To || animationDirection == UIAnimationDirection.RelativeTo) {
                TargetPosition = position;
            } else {
                TargetPosition = CurrentPosition;
                CurrentPosition = position;
            }
            
            if(savePosition) {
                SavedPosition = TargetPosition;
            }
            
            if(instant) {
                CurrentPosition = TargetPosition;
                Moving = false;
                return;
            }

            var animation = GetAnimation(UIAnimationType.Move);
            
            _moveCoroutine = this.CreateCoroutine(MoveCoroutine(animation));
            _moveCoroutine.OnFinished += stopped => {
                if(!stopped) {
                    CurrentPosition = TargetPosition;
                }
                
                Moving = false;

                if(callback != null) {
                    callback();
                }
            };
        }

        protected IEnumerator MoveCoroutine(UIAnimation animation) {
            var movementProgress = 0.0f;
            var startingPosition = CurrentPosition;

            while(movementProgress <= 1) {
                CurrentPosition = startingPosition.EaseTo(TargetPosition, movementProgress, animation.easeFunction);
                
                movementProgress += Time.deltaTime / animation.Duration;
                yield return null;
            }
        }
        
        
        // Resize - similar to the resizable window - pick an edge and resize from there
        
        // Fade
        
        // Rotate

        public void Reset() {
            if(_moveCoroutine != null && _moveCoroutine.Running) {
                _moveCoroutine.Stop();
            }

            CurrentPosition = SavedPosition;
        }
        
        protected static UIManager UIManager {
            get { return Singleton.Get<UIManager>(); }
        }

        // Handle Note movement (from network calls) to make it smooth; Move type component; make the note component honor it
        // TODO register the component in the UIManager; intercept resizing/movement calls to animate them
        // TODO transform extension methods that move/resize objects using the UIManager (thus allowing the resizing/movement to be intercepted and an animation to be played)
    }
}