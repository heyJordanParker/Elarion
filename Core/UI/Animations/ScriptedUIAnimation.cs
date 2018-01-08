using UnityEngine;

namespace Elarion.UI.Animations {
    // TODO remove the MonoBehavior inheritance
    // TODO create a ScriptableObject and a Component that have a ScriptedUIAnimation (or Interface) field
    // TODO create an interface for Unity Objects containing an animation
    // e.g. StoredUIAnimation : ScriptableObject, IUIAnimationContainer { public Animation { get; }}
    // e.g. RuntimeUIAnimation : MonoBehavior, IUIAnimationContainer { public Animation { get; }}
    // register those components in the animator
    // e.g. public StoredUIAnimation[] storedAnimations;
    // e.g. public RuntimeUIAnimation[] runtimeAnimations;
    // add a register function for arbitrary animations that inherit the interface
    
    // TODO create a MovementTweener/RotationTweener classes that cache movement/rotation in LateUpdate and tween to any new position (you can update and they'll override before the frame renders)
    
    /// <summary>
    /// Extend this class to create custom animations via code
    /// </summary>
    public abstract class ScriptedUIAnimation : MonoBehaviour {
        
        [SerializeField]
        public UIAnimationType type;
        
        public abstract void StartAnimation(UIPanel panel);

        public abstract void UpdateAnimation(float progress);

        public abstract void StopAnimation();
    }
}