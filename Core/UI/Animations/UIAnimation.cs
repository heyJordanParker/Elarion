using System;
using Elarion.Attributes;
using UnityEngine;

namespace Elarion.UI.Animations {
    [Serializable]
    public class UIAnimation : ScriptableObject {
        // TODO User-friendly easing - use user-friendly names and a fewer easing functions to make picking one simpler

        // TODO maximize type - starts as 1x1 square in one of the corners (or custom position) and maximizes to the whole screen; optionally use a circular mask

        // TODO Maximize pick a position from an enum - TopLeft, TopCenter, TopRight... BottomRight, Custom; Custom reveals a Vector2 field to input the correct position
        // TODO resize + fade (resize from an icon to fullscreen & fade between icon & screen)

        // cap off-screen movement to Screen size? (this will allow you to setup vectors for movement without worrying about the varying screen size - if you want the object to move offscreen, just setup a pretty large number) 

        [Tooltip("Transition Configuration")]
        public UITransitionType type = UITransitionType.AplhaFade;

        [ConditionalVisibility("type == UITransitionType.Slide")]
        public SlideDirection slideDirection = SlideDirection.Left;

        [SerializeField]
        private UIAnimationDuration _duration = UIAnimationDuration.Normal;

        [SerializeField, ConditionalVisibility(
            "_duration == UIAnimationDuration.Custom")]
        private float _customDuration = .75f;

        [ConditionalVisibility("type != UITransitionType.Inherit, type != UITransitionType.None")]
        public Ease easeFunction = Ease.Linear;

        public float Duration {
            get {
                if(_duration == UIAnimationDuration.Custom) {
                    return _customDuration;
                }

                return (int) _duration / 100f;
            }
        }

        public int animationPriority = 1;
        
        // effects?

        public UIAnimationType animationType;

        public float positionXDelta; // percentage? to move anchors to the side/screen - direction based on sign
        public float positionYDelta;
        
        public float sizeDelta; // percentage to increase/decrease size (based on sign)

        // should the target be reset to its' initial position/rotation/etc
        public bool resetTarget = true;
        

        // animate min, max anchors for movement
        // animate scale and pivot for size
        // animate alpha

        // animation priority - render static elements and then animation elements based on priority; control this in the UIManager, maybe use the panels priority to achieve that

        // remove color fade - do that by fading to a black/white scene and then to the other

        // preset animations (slides and similar)
        // custom animation (whatever the user sets)
        
        public void Animate(UIAnimator animator, RectTransform target) {
            // towards screen (aka should it be reversed)
        }
    }
}