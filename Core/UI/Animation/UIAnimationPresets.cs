using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Elarion.UI.Animation {
    public partial class UIAnimation {
        
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
            PopOut,
            Maximize
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
            public bool overrideParentAnchors = false;
            public Vector2 overrideParentAnchorMin = new Vector2(0.5f, 0.5f);
            public Vector2 overrideParentAnchorMax = new Vector2(0.5f, 0.5f);
        }

        private class FadePreset {
            public bool animateAlpha = false;
            public UIAnimationDirection alphaAnimationDirection = UIAnimationDirection.From;
            public float alphaDelta = 0;
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
        
        #if UNITY_EDITOR
        private void OnValidate() {
            if(MovementPresets.ContainsKey(_movement)) {
                var preset = MovementPresets[_movement];
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

                    animateRotation = preset.animateRotation;
                    rotationAnimationDirection = GetAnimationDirection(preset.relativeRotationMovement);
                    rotationDelta = preset.rotationDelta;

                    overrideParentAnchors = preset.overrideParentAnchors;
                    overrideParentAnchorMin = preset.overrideParentAnchorMin;
                    overrideParentAnchorMax = preset.overrideParentAnchorMax;

                    if(preset.animatePosition || preset.animateAnchors || preset.animateRotation || preset.animateSize) {
                        overrideParentAnchors = true;
                        overrideParentAnchorMin = new Vector2(0.5f, 0.5f);
                        overrideParentAnchorMax = new Vector2(0.5f, 0.5f);
                        overrideAnimationPriority = true;
                        animationPriority = DefaultAnimationPriority + 5;
                    } else {
                        overrideAnimationPriority = false;
                        animationPriority = DefaultAnimationPriority;
                    }
                }
            }
            
            if(FadePresets.ContainsKey(_fade)) {
                var preset = FadePresets[_fade];
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
                                animateSize = true, 
                                relativeSizeMovement = false, 
                                sizeDelta = new Vector2(0, 0)
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
    }
}