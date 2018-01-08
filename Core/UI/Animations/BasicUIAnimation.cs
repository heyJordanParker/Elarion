using System;
using Elarion.Extensions;
using UnityEngine;

namespace Elarion.UI.Animations {
    public abstract class BasicUIAnimation : MonoBehaviour {
        [Flags]
        public enum AnimationTrigger {
            Manual = 0 << 0,
            OnEnable = 1 << 0,
            OnDisable = 1 << 1,
        }

        public bool loopAnimation;

        public AnimationTrigger animationTrigger;

        protected virtual void OnEnable() {
            if(animationTrigger.HasFlag<AnimationTrigger>(AnimationTrigger.OnEnable)) {
                Animate();
            } 
        }

        protected virtual void OnDisable() {
            if(animationTrigger.HasFlag(AnimationTrigger.OnDisable)) {
                Animate();
            } 
        }

        protected abstract void Animate();

    }
}