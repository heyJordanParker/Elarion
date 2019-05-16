using System;
using System.Collections;
using UnityEngine;

namespace Elarion.Common.Extensions {
    public static class ActionExtensions {
        public static IEnumerator ToIEnumerator(this Action action) {
            action(); 
            yield return null;
        }

        public static IEnumerator ToDelayedIEnumerator(this Action action, float delay) {
            yield return new WaitForSeconds(delay);
            action();
            yield return null;
        }

        public static IEnumerator ToUpdateIEnumerator(this Action action, float updateTime) {
            while(true) {
                action();
                yield return updateTime <= 0 ? null : new WaitForSeconds(updateTime);
            }
        }
    }
}