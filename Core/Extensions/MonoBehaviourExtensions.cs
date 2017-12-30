using System;
using System.Collections;
using UnityEngine;

namespace Elarion.Extensions {
    public static class MonoBehaviourExtensions {

        public static ECoroutine CreateCoroutine(this MonoBehaviour monoBehaviour, IEnumerator routine,
            bool autoStart = true, bool autoPause = true) {
            return new ECoroutine(routine, monoBehaviour, autoStart, autoPause);
        }
        
        public static ECoroutine CreateActionCoroutine(this MonoBehaviour monoBehaviour, Action action,
            bool autoStart = true, bool autoPause = true) {
            return monoBehaviour.CreateCoroutine(action.ToIEnumerator(), autoStart, autoPause);
        }
               
        public static ECoroutine CreateDelayedCoroutine(this MonoBehaviour monoBehaviour, Action action, float delay,
            bool autoStart = true, bool autoPause = true) {
            return monoBehaviour.CreateCoroutine(action.ToDelayedIEnumerator(delay), autoStart, autoPause);
        }
        
        public static ECoroutine CreateUpdateCoroutine(this MonoBehaviour monoBehaviour, Action action, float delay,
            bool autoStart = true, bool autoPause = true) {
            return monoBehaviour.CreateCoroutine(action.ToUpdateIEnumerator(delay), autoStart, autoPause);
        }
        
    }
}