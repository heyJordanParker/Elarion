using System;
using Elarion.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.UI {
    [RequireComponent(typeof(UIPanel))]
    public class UIAnimator : BasicUIElement {
        
        // TODO custom inspector
        // array of animations; show an inspector for a single item when multipleAnimations is checked (still use the array underneath though)
        // TODO make the same inspector for UIEffects
        // basically, make the UIEffect class similar to this structure; maybe even make it a ScriptableObject
        
        [Serializable]
        public class UpdateAnimationEvent : UnityEvent<float> { }

        [Serializable]
        private enum AnimationType {
            Simple,
            Scripted
        }

        private enum UIAnimationTrigger {
            OnShow,
            OnHide,
            //etc
        }

        [SerializeField]
        private bool _multipleAnimations;

        [SerializeField]
        private UIAnimationTrigger _trigger;
        
        [SerializeField]
        private AnimationType _type;

        [SerializeField, ConditionalVisibility("_type == AnimationType.Simple")]
        private UIAnimation _animation;
        [SerializeField, ConditionalVisibility("_type == AnimationType.Scripted")]
        private UpdateAnimationEvent _scriptedAnimation; 

        // Handle Note movement (from network calls) to make it smooth; Move type component; make the note component honor it
        // TODO register the component in the UIManager; intercept resizing/movement calls to animate them
        // TODO transform extension methods that move/resize objects using the UIManager (thus allowing the resizing/movement to be intercepted and an animation to be played)
    }
}