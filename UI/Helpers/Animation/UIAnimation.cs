using System;
using System.Collections.Generic;
using Elarion;
using Elarion.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Elarion.UI.Helpers.Animation {

    [Serializable]
    [CreateAssetMenu(order = 250)]
    public class UIAnimation : ScriptableObject {

        // TODO custom inspector
        
        // TODO animate scale (pop-in animations generally look better when the component is scaled instead of resized)
        
        // TODO Animate interface (containing the animate method); maybe add a target field (useful for other scripts)
        
        
        #region AnimationPresets

                
        [Serializable]
        private enum AnimationMovementPreset {
            NoMovement = 0,
            Custom = int.MaxValue,
            SlideLeft = 1,
            SlideRight,
            SlideUp,
            SlideDown,
            PopIn,
            PopOut,
            Maximize,
            Minimize
        }

        [Serializable]
        private enum AnimationFadePreset {
            NoFade = 0,
            Custom = int.MaxValue,
            FadeIn = 1,
            FadeOut
        }

        [Serializable]
        private class MovementPreset {
            public bool animatePosition = false;
            public bool relativePositionMovement = true;
            public Vector2 positionDelta = Vector2.zero;
            public bool animateAnchors = false;
            public bool relativeAnchorsMovement = false;
            public Vector2 minAnchorDelta = Vector2.zero;
            public Vector2 maxAnchorDelta = Vector2.zero;
            public bool animateSize = false;
            public bool relativeSizeMovement = true;
            public Vector2 sizeDelta = Vector2.zero;
            public bool animateScale = false;
            public bool relativeScaleMovement = true;
            public Vector2 scaleDelta = Vector2.zero;
            public bool animateRotation = false;
            public bool relativeRotationMovement = true;
            public Vector3 rotationDelta = Vector3.zero;
            public bool overrideParentAnchors = false;
            public Vector2 overrideParentAnchorMin = new Vector2(0.5f, 0.5f);
            public Vector2 overrideParentAnchorMax = new Vector2(0.5f, 0.5f);
        }

        private class FadePreset {
            public bool animateAlpha = false;
            public UIAnimationDirection alphaAnimationDirection = UIAnimationDirection.From;
            public float alphaDelta = 0;
        }
        
        // Convenience fields for the editor, can't remove them via conditional compilation because the ScriptableObject serialization busts
#pragma warning disable CS0414
        
        [Header("Animation Configuration")]
        [SerializeField]
        [FormerlySerializedAs("_movement")]
        private AnimationMovementPreset _movementPreset = AnimationMovementPreset.NoMovement;
                
        [SerializeField]
        [FormerlySerializedAs("_fade")]
        private AnimationFadePreset _fadePreset = AnimationFadePreset.FadeIn;
        
#pragma warning restore CS0414

        
#if UNITY_EDITOR
        private void OnValidate() {
            if(MovementPresets.ContainsKey(_movementPreset)) {
                var preset = MovementPresets[_movementPreset];
                if(preset != null) {
                    UnityEditor.Undo.RecordObject(this, "Changing Movement Preset");
                    
                    animatePosition = preset.animatePosition;
                    positionAnimationDirection = GetAnimationDirection(preset.relativePositionMovement);
                    positionDelta = preset.positionDelta;

                    animateAnchors = preset.animateAnchors;
                    anchorsAnimationDirection = GetAnimationDirection(preset.relativeAnchorsMovement);
                    minAnchorDelta = preset.minAnchorDelta;
                    maxAnchorDelta = preset.maxAnchorDelta;

                    animateSize = preset.animateSize;
                    sizeAnimationDirection = GetAnimationDirection(preset.relativeSizeMovement);
                    sizeDelta = preset.sizeDelta;
                    
                    animateScale = preset.animateScale;
                    scaleAnimationDirection = GetAnimationDirection(preset.relativeScaleMovement);
                    scaleDelta = preset.scaleDelta;

                    animateRotation = preset.animateRotation;
                    rotationAnimationDirection = GetAnimationDirection(preset.relativeRotationMovement);
                    rotationDelta = preset.rotationDelta;

                    overrideParentAnchors = preset.overrideParentAnchors;
                    overrideParentAnchorMin = preset.overrideParentAnchorMin;
                    overrideParentAnchorMax = preset.overrideParentAnchorMax;

                    if(preset.animatePosition || preset.animateAnchors || preset.animateRotation || preset.animateSize || preset.animateScale) {
                        overrideParentAnchors = true;
                        overrideParentAnchorMin = new Vector2(0.5f, 0.5f);
                        overrideParentAnchorMax = new Vector2(0.5f, 0.5f);
                    }
                }
            }
            
            if(FadePresets.ContainsKey(_fadePreset)) {
                var preset = FadePresets[_fadePreset];
                if(preset != null) {
                    UnityEditor.Undo.RecordObject(this, "Changing Fade Preset");

                    animateAlpha = preset.animateAlpha;
                    alphaAnimationDirection = preset.alphaAnimationDirection;
                    alphaDelta = preset.alphaDelta;
                }
            }
        }
        
        private static Dictionary<AnimationMovementPreset, MovementPreset> _movementPresets;
        private static Dictionary<AnimationFadePreset, FadePreset> _fadePresets;

        private static Dictionary<AnimationMovementPreset, MovementPreset> MovementPresets {
            get {
                if(_movementPresets == null) {
                    _movementPresets = new Dictionary<AnimationMovementPreset, MovementPreset> {
                        {
                            AnimationMovementPreset.NoMovement, 
                            new MovementPreset()
                        }, {
                            AnimationMovementPreset.SlideLeft, 
                            new MovementPreset {
                                animateAnchors = true, 
                                relativeAnchorsMovement = true, 
                                minAnchorDelta = new Vector2(-1, 0), 
                                maxAnchorDelta = new Vector2(-1, 0)
                            }
                        }, {
                            AnimationMovementPreset.SlideRight, 
                            new MovementPreset {
                                animateAnchors = true, 
                                relativeAnchorsMovement = true, 
                                minAnchorDelta = new Vector2(1, 0), 
                                maxAnchorDelta = new Vector2(1, 0)
                            }
                        }, {
                            AnimationMovementPreset.SlideUp, 
                            new MovementPreset {
                                animateAnchors = true, 
                                relativeAnchorsMovement = true, 
                                minAnchorDelta = new Vector2(0, 1), 
                                maxAnchorDelta = new Vector2(0, 1)
                            }
                        }, {
                            AnimationMovementPreset.SlideDown, 
                            new MovementPreset {
                                animateAnchors = true, 
                                relativeAnchorsMovement = true, 
                                minAnchorDelta = new Vector2(0, -1), 
                                maxAnchorDelta = new Vector2(0, -1)
                            }
                        }, {
                            AnimationMovementPreset.PopIn, 
                            new MovementPreset {
                                animateSize = true, 
                                relativeSizeMovement = true, 
                                sizeDelta = new Vector2(150, 150) 
                            }
                        }, {
                            AnimationMovementPreset.PopOut, 
                            new MovementPreset {
                                animateSize = true, 
                                relativeSizeMovement = true, 
                                sizeDelta = new Vector2(-250, -250)
                            }
                        }, {
                            AnimationMovementPreset.Maximize, 
                            new MovementPreset {
                                animateScale = true, 
                                relativeScaleMovement = false, 
                                scaleDelta = new Vector2(1, 1)
                            }
                        }, {
                            AnimationMovementPreset.Minimize, 
                            new MovementPreset {
                                animateScale = true, 
                                relativeScaleMovement = false, 
                                scaleDelta = new Vector2(0, 0)
                            }
                        },
                    };
                }
                return _movementPresets;
            }
        }

        private static Dictionary<AnimationFadePreset, FadePreset> FadePresets {
            get {
                if(_fadePresets == null) {
                    _fadePresets = new Dictionary<AnimationFadePreset, FadePreset> {
                        {AnimationFadePreset.NoFade, new FadePreset()},
                        {AnimationFadePreset.FadeIn, new FadePreset {animateAlpha = true, alphaAnimationDirection = UIAnimationDirection.From, alphaDelta = 0}},
                        {AnimationFadePreset.FadeOut, new FadePreset {animateAlpha = true, alphaAnimationDirection = UIAnimationDirection.To, alphaDelta = 0}},
                    };
                }
                return _fadePresets;
            }
        }
#endif

        #endregion
        
        [Serializable]
        private enum AnimationDirection {
            From = UIAnimationDirection.From,
            To = UIAnimationDirection.To
        }

        [SerializeField]
        [ConditionalVisibility("_movementPreset != AnimationMovementPreset.NoMovement || AnimationMovementPreset.Custom")]
        private AnimationDirection _movementDirection = AnimationDirection.From;

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
        public bool animateScale = false;
        
        [ConditionalVisibility("animateScale", "_movementPreset == AnimationMovementPreset.Custom")]
        public UIAnimationDirection scaleAnimationDirection = UIAnimationDirection.RelativeTo;
        
        [ConditionalVisibility("animateScale", "_movementPreset == AnimationMovementPreset.Custom")]
        public Vector2 scaleDelta = Vector2.zero;

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
        [Header("Advanced Options (Movement)")]
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

        public float Delay => _delay;

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

        private UIAnimationDirection GetAnimationDirection(bool isRelative) {
            if(!isRelative) {
                return (UIAnimationDirection) _movementDirection;
            }

            return _movementDirection == AnimationDirection.To ? UIAnimationDirection.RelativeTo : UIAnimationDirection.RelativeFrom;
        }
        
        public void Animate(UIAnimator animator, Action callback = null, UIAnimationOptions animationOptions = null) {
            if(animationOptions == null) {
                animationOptions = new UIAnimationOptions(savePosition, Duration == 0, _customEasingFunction, Duration, Delay);
            }
            
            if(animatePosition) {
                animator.Move(positionDelta, positionAnimationDirection, callback, animationOptions);
                callback = null;
            }
            
            if(animateAnchors) {
                animator.MoveAnchors(minAnchorDelta, maxAnchorDelta, anchorsAnimationDirection, callback, animationOptions);
                callback = null;
            }
            
            if(animateRotation) {
                animator.Rotate(rotationDelta, rotationAnimationDirection, callback, animationOptions);
                callback = null;
            }
            
            if(animateSize) {
                animator.Resize(sizeDelta, sizeAnimationDirection, callback, animationOptions);
                callback = null;
            }

            if(animateScale) {
                animator.Scale(scaleDelta, scaleAnimationDirection, callback, animationOptions);
                callback = null;
            }
            
            if(animateAlpha) {
                animator.Fade(alphaDelta, alphaAnimationDirection, callback, animationOptions);
                callback = null;
            }

            callback?.Invoke();
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
            
            if(animateScale)
                animator.ScaleTweener.StopTween(reset);
            
            if(animateAlpha)
                animator.AlphaTweener.StopTween(reset);
        }

    }
}