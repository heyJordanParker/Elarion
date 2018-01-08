using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.UI.Animations {
    public class EventBasedUIAnimation : ScriptedUIAnimation {
        [Serializable]
        public class UpdateAnimationEvent : UnityEvent<float> { }
            
        [Serializable]
        public class PlayAnimationEvent : UnityEvent<UIPanel> { }
        
        [SerializeField]
        public PlayAnimationEvent onAnimationStart; 
        [SerializeField]
        public UpdateAnimationEvent onAnimationUpdate;
        [SerializeField]
        public UnityEvent onAnimationStop;

        public override void StartAnimation(UIPanel panel) {
            onAnimationStart.Invoke(panel);
        }

        public override void UpdateAnimation(float progress) {
            onAnimationUpdate.Invoke(progress);
        }

        public override void StopAnimation() {
            onAnimationStop.Invoke();
        }
    }
}