using System;
using System.Collections;
using Elarion.Common.Coroutines;
using UnityEngine;

namespace Elarion.Common.Extensions {
    public static class MonoBehaviourExtensions {
        public static ECoroutine CreateCoroutine(this MonoBehaviour monoBehaviour, IEnumerator routine,
            bool autoStart = true) {
            return new ECoroutine(routine, monoBehaviour, autoStart);
        }
        
        public static ECoroutine CreateActionCoroutine(this MonoBehaviour monoBehaviour, Action action,
            bool autoStart = true) {
            return monoBehaviour.CreateCoroutine(action.ToIEnumerator(), autoStart);
        }
               
        public static ECoroutine CreateDelayedCoroutine(this MonoBehaviour monoBehaviour, Action action, float delay,
            bool autoStart = true) {
            return monoBehaviour.CreateCoroutine(action.ToDelayedIEnumerator(delay), autoStart);
        }
        
        public static ECoroutine CreateUpdateCoroutine(this MonoBehaviour monoBehaviour, Action action, float delay,
            bool autoStart = true) {
            return monoBehaviour.CreateCoroutine(action.ToUpdateIEnumerator(delay), autoStart);
        }
        
    }
}