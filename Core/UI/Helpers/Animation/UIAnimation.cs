using System;
using Elarion.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Elarion.UI.Helpers.Animation {

    [Serializable]
    [CreateAssetMenu(order = 250)]
    public partial class UIAnimation : ScriptableObject {

        // TODO animation sound
        
        // TODO custom inspector
        
        // TODO animate scale (pop-in animations generally look better when the component is scaled instead of resized)
        
        // TODO Animate interface (containing the animate method); maybe add a target field (useful for other scripts)

        #region PresetConfiguration
        
        #if UNITY_EDITOR
        [Header("Animation Configuration")]
        [SerializeField]
        [FormerlySerializedAs("_movement")]
        private AnimationMovementPreset _movementPreset = AnimationMovementPreset.NoMovement;
        #endif
        
        [SerializeField]
        [ConditionalVisibility("_movementPreset != AnimationMovementPreset.NoMovement || AnimationMovementPreset.Custom")]
        private AnimationPresetDirection _movementDirection = AnimationPresetDirection.From;
        
        #if UNITY_EDITOR
        [SerializeField]
        [FormerlySerializedAs("_fade")]
        private AnimationFadePreset _fadePreset = AnimationFadePreset.FadeIn;
        #endif
        
        #endregion

        [Space(10)]
        [SerializeField]
        [Tooltip("Delay in seconds")]
        private float _delay = 0f;
        
        [SerializeField]
        [FormerlySerializedAs("_duration")]
        private UIAnimationDuration _durationPreset = UIAnimationDuration.Normal;

        [SerializeField, ConditionalVisibility(
             "_durationPreset == UIAnimationDuration.Custom")]
        private float _customDuration = .75f;

        [SerializeField]
        [FormerlySerializedAs("_easeFunction")]
        private UIAnimationEase _easingFunctionPreset = UIAnimationEase.Smooth; 
        
        [SerializeField, ConditionalVisibility(
             "_easingFunctionPreset == UIAnimationEase.Custom")]
        [FormerlySerializedAs("_customEaseFunction")]
        private Ease _customEasingFunction = Ease.Linear;

        [Header("Movement Options")]
        [ConditionalVisibility(enableConditions: "_movementPreset == AnimationMovementPreset.Custom")]
        public bool animatePosition = false;

        [ConditionalVisibility("animatePosition", "_movementPreset == AnimationMovementPreset.Custom")]
        public UIAnimationDirection positionAnimationDirection = UIAnimationDirection.RelativeTo;

        [ConditionalVisibility("animatePosition", "_movementPreset == AnimationMovementPreset.Custom", order = 1)]
        public Vector2 positionDelta = Vector2.zero;

        [ConditionalVisibility(enableConditions: "_movementPreset == AnimationMovementPreset.Custom")]
        public bool animateAnchors = false;

        [ConditionalVisibility("animateAnchors", "_movementPreset == AnimationMovementPreset.Custom")]
        public UIAnimationDirection anchorsAnimationDirection = UIAnimationDirection.From;

        [ConditionalVisibility("animateAnchors", "_movementPreset == AnimationMovementPreset.Custom")]
        public Vector2 minAnchorDelta = Vector2.zero;
        
        [ConditionalVisibility("animateAnchors", "_movementPreset == AnimationMovementPreset.Custom")]
        public Vector2 maxAnchorDelta = Vector2.zero;

        [ConditionalVisibility(enableConditions: "_movementPreset == AnimationMovementPreset.Custom")]
        public bool animateSize = false;
        
        [ConditionalVisibility("animateSize", "_movementPreset == AnimationMovementPreset.Custom")]
        public UIAnimationDirection sizeAnimationDirection = UIAnimationDirection.RelativeTo;
        
        [ConditionalVisibility("animateSize", "_movementPreset == AnimationMovementPreset.Custom")]
        public Vector2 sizeDelta = Vector2.zero;

        [ConditionalVisibility(enableConditions: "_movementPreset == AnimationMovementPreset.Custom")]
        public bool animateRotation = false;
        
        [ConditionalVisibility("animateRotation", "_movementPreset == AnimationMovementPreset.Custom")]
        public UIAnimationDirection rotationAnimationDirection = UIAnimationDirection.RelativeTo;
        
        [ConditionalVisibility("animateRotation", "_movementPreset == AnimationMovementPreset.Custom")]
        public Vector3 rotationDelta = Vector3.zero;
        
        [Space(10)]
        [Header("Fade Options")]
        [ConditionalVisibility(enableConditions: "_fadePreset == AnimationFadePreset.Custom")]
        public bool animateAlpha = false;
        
        [ConditionalVisibility("animateAlpha", "_fadePreset == AnimationFadePreset.Custom")]
        public UIAnimationDirection alphaAnimationDirection = UIAnimationDirection.From;
        
        [ConditionalVisibility("animateAlpha", "_fadePreset == AnimationFadePreset.Custom")]
        public float alphaDelta = 0;
        
        [Space(10)]
        [Header("Advanced Options")]
        [ConditionalVisibility(enableConditions: "_movementPreset == AnimationMovementPreset.Custom")]
        public bool overrideParentAnchors = false;
        
        [ConditionalVisibility("overrideParentAnchors", "_movementPreset == AnimationMovementPreset.Custom")]
        public Vector2 overrideParentAnchorMin = new Vector2(0.5f, 0.5f);
        [ConditionalVisibility("overrideParentAnchors", "_movementPreset == AnimationMovementPreset.Custom")]
        public Vector2 overrideParentAnchorMax = new Vector2(0.5f, 0.5f);

        
        // TODO move this to the scripts that call/chain animations - it doesn't need to be here
        [HideInInspector]
        [Tooltip("Save the position after the movement finishes.", order = 0)]
        [ConditionalVisibility("_movementPreset == AnimationMovementPreset.Custom", order = 1)]
        public bool savePosition = false;

        public float Delay {
            get { return _delay; }
        }
        
        public float Duration {
            get {
                if(_durationPreset == UIAnimationDuration.Custom) {
                    return _customDuration;
                }

                return (int) _durationPreset / 100f;
            }
        }
        
        public Ease Ease {
            get {
                if(_easingFunctionPreset == UIAnimationEase.Custom) {
                    return _customEasingFunction;
                }

                return (Ease) _easingFunctionPreset;
            }
        }

        private UIAnimationOptions AnimationOptions {
            get {
                return new UIAnimationOptions(savePosition, Duration == 0, _customEasingFunction, Duration, Delay);
            }
        }
        
        public void Animate(UIAnimator animator, Action callback = null) {
            if(animatePosition) {
                animator.Move(positionDelta, positionAnimationDirection, callback, AnimationOptions);
                callback = null; // to avoid multiple calls of the callback
            }
            
            if(animateAnchors) {
                animator.MoveAnchors(minAnchorDelta, maxAnchorDelta, anchorsAnimationDirection, callback, AnimationOptions);
                callback = null; // to avoid multiple calls of the callback
            }
            
            if(animateRotation) {
                animator.Rotate(rotationDelta, rotationAnimationDirection, callback, AnimationOptions);
                callback = null; // to avoid multiple calls of the callback
            }
            
            if(animateSize) {
                animator.Resize(sizeDelta, sizeAnimationDirection, callback, AnimationOptions);
                callback = null; // to avoid multiple calls of the callback
            }
            
            if(animateAlpha) {
                animator.Fade(alphaDelta, alphaAnimationDirection, callback, AnimationOptions);
                callback = null; // to avoid multiple calls of the callback
            }
        }

        public void Stop(UIAnimator animator, bool reset = false) {
            if(animatePosition)
                animator.PositionTweener.StopTween(reset);
            
            if(animateAnchors)
                animator.AnchorsTweener.StopTween(reset);
            
            if(animateRotation)
                animator.RotationTweener.StopTween(reset);
            
            if(animateSize)
                animator.SizeTweener.StopTween(reset);
            
            if(animateAlpha)
                animator.AlphaTweener.StopTween(reset);
        }
    }
}