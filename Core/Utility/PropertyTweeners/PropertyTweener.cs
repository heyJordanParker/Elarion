using System;
using System.Collections;
using Elarion.Extensions;
using Elarion.UI;
using Elarion.UI.Animation;
using UnityEngine;

namespace Elarion.Utility.PropertyTweeners {

    public abstract class PropertyTweener<TProperty, TTarget> where TProperty : struct {
        
        protected ECoroutine tweenerCoroutine;

        public TProperty SavedValue { get; protected set; }
        public abstract TProperty CurrentValue { get; protected set; }
        public TProperty TargetValue { get; protected set; }
        public bool Tweening { get; set; }

        public TTarget Target {
            get { return _target; }
            set {
                _target = value;
                SaveProperty();
            }
        }

        private TTarget _target;
        private readonly MonoBehaviour _owner;

        protected PropertyTweener(MonoBehaviour owner) {
            _owner = owner;
        }
        
        public void Tween(TProperty value, UIAnimationDirection animationDirection = UIAnimationDirection.From,
            Action callback = null, UIAnimationOptions animationOptions = null) {

            if(animationOptions == null) {
                animationOptions = new UIAnimationOptions();
            }
            
            if(tweenerCoroutine != null && tweenerCoroutine.Running) {
                StopTween();
                tweenerCoroutine.OnFinished += b => {
                    Tween(value, animationDirection, callback, animationOptions);
                };
                return;
            }
            
            if(animationDirection == UIAnimationDirection.RelativeTo || animationDirection == UIAnimationDirection.RelativeFrom) {
                value = AddValues(value, TargetValue);
            }
            
            if(animationDirection == UIAnimationDirection.To || animationDirection == UIAnimationDirection.RelativeTo) {
                TargetValue = value;
            } else {
                TargetValue = CurrentValue;
                CurrentValue = value;
            }
            
            if(animationOptions.SavePosition) {
                SavedValue = TargetValue;
            }
            
            if(animationOptions.Instant) {
                CurrentValue = TargetValue;
                return;
            }

            Tweening = true;

            tweenerCoroutine = _owner.CreateCoroutine(TweenCoroutine(animationOptions.EaseFunction, animationOptions.Duration));
            tweenerCoroutine.OnFinished += stopped => {
                if(!stopped) {
                    CurrentValue = TargetValue;
                }

                Tweening = false;
                
                if(callback != null) {
                    callback();
                }
            };
        }

        public void StopTween(bool reset = false) {
            if(tweenerCoroutine != null && tweenerCoroutine.Running) {
                tweenerCoroutine.Stop();
            }
            
            if(reset) {
                TargetValue = CurrentValue = SavedValue;
            }
        }

        public void SaveProperty() {
            SavedValue = TargetValue = CurrentValue;
        }

        public void ResetProperty() {
            StopTween(true);
        }
        
        protected IEnumerator TweenCoroutine(Ease ease, float duration) {
            var movementProgress = 0.0f;
            var startingAnchors = CurrentValue;
            
            while(movementProgress <= 1) {
                CurrentValue = UpdateValue(startingAnchors, movementProgress, ease);
                
                movementProgress += Time.deltaTime / duration;
                yield return null;
            }
        }
        
        protected abstract TProperty UpdateValue(TProperty startingValue, float progress, Ease ease);

        // this is needed because generics do not support addition and the dynamic keyword isn't available in Unity
        protected abstract TProperty AddValues(TProperty value1, TProperty value2);
        
    }
}