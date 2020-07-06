using System;
using System.Collections;
using Elarion.Coroutines;
using Elarion.Extensions;
using Elarion.UI.Helpers.Animation;
using UnityEngine;

namespace Elarion.UI.PropertyTweeners {

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

            tweenerCoroutine = _owner.CreateCoroutine(TweenCoroutine(animationOptions.Ease, animationOptions.Duration, animationOptions.Delay));
            tweenerCoroutine.OnFinished += stopped => {
                if(!stopped) {
                    CurrentValue = TargetValue;
                }

                Tweening = false;

                callback?.Invoke();
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

        public void ResetPropertyGraceful(float duration = 0.3f) {
            Tween(SavedValue, UIAnimationDirection.To, ResetProperty, new UIAnimationOptions(duration: duration));
        }
        
        protected virtual IEnumerator TweenCoroutine(Ease ease, float duration, float delay) {
            var movementProgress = 0.0f;
            var startingValue = CurrentValue;

            if(delay > 0) {
                yield return new WaitForSeconds(delay);
            }

            while(movementProgress <= 1) {
                CurrentValue = UpdateValue(startingValue, movementProgress, ease);

                movementProgress += Time.unscaledDeltaTime / duration;
                yield return null;
            }

            CurrentValue = UpdateValue(startingValue, 1, ease);
        }
        
        protected abstract TProperty UpdateValue(TProperty startingValue, float progress, Ease ease);

        // this is needed because the dynamic keyword isn't available in Unity IL2CPP compilation
        protected abstract TProperty AddValues(TProperty value1, TProperty value2);
        
    }
}