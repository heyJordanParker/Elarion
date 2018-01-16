using System;
using System.Collections.Generic;
using System.Net;
using Elarion.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.UI.Animation {

    // TODO a custom inspector to enable both conditional naming and conditional visibility
    
    // Animations modify the properties of an object
    [Serializable]
    public class UIAnimation : ScriptableObject {

        [Serializable]
        private enum AnimationPresetDirection {
            From = UIAnimationDirection.From,
            To = UIAnimationDirection.To
        }
        
        [Serializable]
        private enum AnimationMovementPreset {
            NoMovement = 0,
            Custom = int.MaxValue,
            SlideLeft = 1,
            SlideRight,
            SlideUp,
            SlideDown,
            PopIn,
            PopOut
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
            public bool animateRotation = false;
            public bool relativeRotationMovement = true;
            public Vector3 rotationDelta = Vector3.zero;
        }

        private class FadePreset {
            public bool animateAlpha = false;
            public UIAnimationDirection alphaAnimationDirection = UIAnimationDirection.From;
            public float alphaDelta = 0;
        }
        
        // TODO Wrap the easing the same way as the duration - a few easy to understand options and a custom one (revealing ALL the options)

        // TODO maximize type - starts as 1x1 square in one of the corners (or custom position) and maximizes to the whole screen; optionally use a circular mask

        // TODO Maximize pick a position from an enum - TopLeft, TopCenter, TopRight... BottomRight, Custom; Custom reveals a Vector2 field to input the correct position
        // TODO resize + fade (resize from an icon to fullscreen & fade between icon & screen)

        // cap off-screen movement to Screen size? (this will allow you to setup vectors for movement without worrying about the varying screen size - if you want the object to move offscreen, just setup a pretty large number) 

        // old
        [HideInInspector]
        public UITransitionType type = UITransitionType.AplhaFade;

        // old
        [HideInInspector]
        public SlideDirection slideDirection = SlideDirection.Left;

        [Header("Animation Configuration")]
        [SerializeField]
        private AnimationMovementPreset _movement = AnimationMovementPreset.NoMovement;
        [SerializeField]
        [ConditionalVisibility("_movement != AnimationMovementPreset.NoMovement || AnimationMovementPreset.Custom")]
        private AnimationPresetDirection _movementDirection = AnimationPresetDirection.From;
        [SerializeField]
        private AnimationFadePreset _fade = AnimationFadePreset.FadeIn;
        
        [Space(10)]
        [SerializeField]
        private UIAnimationDuration _duration = UIAnimationDuration.Normal;

        [SerializeField, ConditionalVisibility(
             "_duration == UIAnimationDuration.Custom")]
        private float _customDuration = .75f;

        [SerializeField]
        private UIAnimationEase _easeFunction = UIAnimationEase.Smooth; 
        
        [SerializeField, ConditionalVisibility(
             "_easeFunction == UIAnimationEase.Custom")]
        private Ease _customEaseFunction = Ease.Linear;

        // effects?

        [Header("Movement Options")]
        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool animatePosition = false;

        [ConditionalVisibility("animatePosition", "_movement == AnimationMovementPreset.Custom")]
        public UIAnimationDirection positionAnimationDirection = UIAnimationDirection.RelativeTo;

        [ConditionalVisibility("animatePosition", "_movement == AnimationMovementPreset.Custom", order = 1)]
        public Vector2 positionDelta = Vector2.zero;

        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool animateAnchors = false;

        [ConditionalVisibility("animateAnchors", "_movement == AnimationMovementPreset.Custom")]
        public UIAnimationDirection anchorsAnimationDirection = UIAnimationDirection.From;

        [ConditionalVisibility("animateAnchors", "_movement == AnimationMovementPreset.Custom")]
        public Vector2 minAnchorDelta = Vector2.zero;
        
        [ConditionalVisibility("animateAnchors", "_movement == AnimationMovementPreset.Custom")]
        public Vector2 maxAnchorDelta = Vector2.zero;

        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool animateSize = false;
        
        [ConditionalVisibility("animateSize", "_movement == AnimationMovementPreset.Custom")]
        public UIAnimationDirection sizeAnimationDirection = UIAnimationDirection.RelativeTo;
        
        [ConditionalVisibility("animateSize", "_movement == AnimationMovementPreset.Custom")]
        public Vector2 sizeDelta = Vector2.zero;

        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool animateRotation = false;
        
        [ConditionalVisibility("animateRotation", "_movement == AnimationMovementPreset.Custom")]
        public UIAnimationDirection rotationAnimationDirection = UIAnimationDirection.RelativeTo;
        
        [ConditionalVisibility("animateRotation", "_movement == AnimationMovementPreset.Custom")]
        public Vector3 rotationDelta = Vector3.zero;
        
        [Space(10)]
        [Header("Fade Options")]
        [ConditionalVisibility(enableConditions: "_fade == AnimationFadePreset.Custom")]
        public bool animateAlpha = false;
        
        [ConditionalVisibility("animateAlpha", "_fade == AnimationFadePreset.Custom")]
        public UIAnimationDirection alphaAnimationDirection = UIAnimationDirection.From;
        
        [ConditionalVisibility("animateAlpha", "_fade == AnimationFadePreset.Custom")]
        public float alphaDelta = 0;
        
        [Space(10)]
        [Header("Advanced Options")]
        public bool advancedOptions = false;
        
        [HideInInspector]
        [Tooltip("Save the position after the movement finishes.", order = 0)]
        [ConditionalVisibility("advancedOptions", order = 1)]
        public bool savePosition = false;

        [ConditionalVisibility("advancedOptions")]
        public bool overrideAnimationPriority = false;
        
        [ConditionalVisibility("advancedOptions, overrideAnimationPriority")]
        public int animationPriority = 10;


        private UIAnimationOptions AnimationOptions {
            get {
                return new UIAnimationOptions(savePosition, Duration == 0, _customEaseFunction, Duration);
            }
        }
        
        public float Duration {
            get {
                if(_duration == UIAnimationDuration.Custom) {
                    return _customDuration;
                }

                return (int) _duration / 100f;
            }
        }
        
        public Ease Ease {
            get {
                if(_easeFunction == UIAnimationEase.Custom) {
                    return _customEaseFunction;
                }

                return (Ease) _easeFunction;
            }
        }

        // animation priority - render static elements and then animation elements based on priority; control this in the UIManager, maybe use the panels priority to achieve that
        // TODO add animation priority to animation presets

        // preset animations (slides and similar)

        // TODO animation interface (containing the animate method); maybe add a target field (useful for other scripts)
        public void Animate(UIAnimator animator, Action callback = null) {
            // TODO animate to saved position?
            
            if(animatePosition)
                animator.Move(positionDelta, positionAnimationDirection, callback, AnimationOptions);
            
            if(animateAnchors)
                animator.MoveAnchors(minAnchorDelta, maxAnchorDelta, anchorsAnimationDirection, callback, AnimationOptions);
            
            if(animateRotation)
                animator.Rotate(rotationDelta, rotationAnimationDirection, callback, AnimationOptions);
            
            if(animateSize)
                animator.Resize(sizeDelta, sizeAnimationDirection, callback, AnimationOptions);
            
            if(animateAlpha)
                animator.Fade(alphaDelta, alphaAnimationDirection, callback, AnimationOptions);
        }
        
        private void OnValidate() {
            if(MovementPresets.ContainsKey(_movement)) {
                var preset = MovementPresets[_movement];
                if(preset != null) {
                    Undo.RecordObject(this, "Changing Movement Preset");
                    
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

                    animateRotation = preset.animateRotation;
                    rotationAnimationDirection = GetAnimationDirection(preset.relativeRotationMovement);
                    rotationDelta = preset.rotationDelta;
                }
            }
            
            if(FadePresets.ContainsKey(_fade)) {
                var preset = FadePresets[_fade];
                if(preset != null) {
                    Undo.RecordObject(this, "Changing Fade Preset");

                    animateAlpha = preset.animateAlpha;
                    alphaAnimationDirection = preset.alphaAnimationDirection;
                    alphaDelta = preset.alphaDelta;
                }
            }
        }

        private UIAnimationDirection GetAnimationDirection(bool isRelative) {
            if(!isRelative) {
                return (UIAnimationDirection) _movementDirection;
            }

            if(_movementDirection == AnimationPresetDirection.To) {
                return UIAnimationDirection.RelativeTo;
            }

            return UIAnimationDirection.RelativeFrom;
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
                        },
                        {
                            AnimationMovementPreset.PopOut, 
                            new MovementPreset {
                                animateSize = true, 
                                relativeSizeMovement = true, 
                                sizeDelta = new Vector2(-250, -250)
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
    }
}