using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Extensions {
    public static class ScrollRectExtensions {

        public static void ScrollToTop(this ScrollRect scrollRect) {
            if(!scrollRect.isActiveAndEnabled) {
                return;
            }
            
            scrollRect.StartCoroutine(ScrollCoroutine(scrollRect, new Vector2(scrollRect.normalizedPosition.x, 1)));
        }

        public static void ScrollToBottom(this ScrollRect scrollRect) {
            if(!scrollRect.isActiveAndEnabled) {
                return;
            }

            scrollRect.StartCoroutine(ScrollCoroutine(scrollRect, new Vector2(scrollRect.normalizedPosition.x, 0)));
        }

        public static void ScrollToLeft(this ScrollRect scrollRect) {
            if(!scrollRect.isActiveAndEnabled) {
                return;
            }

            scrollRect.StartCoroutine(ScrollCoroutine(scrollRect, new Vector2(0, scrollRect.normalizedPosition.y)));
        }

        public static void ScrollToRight(this ScrollRect scrollRect) {
            if(!scrollRect.isActiveAndEnabled) {
                return;
            }

            scrollRect.StartCoroutine(ScrollCoroutine(scrollRect, new Vector2(1, scrollRect.normalizedPosition.y)));
        }

        // This only works after the ScrollRect has already initialized so we're delaying it by one frame
        private static IEnumerator ScrollCoroutine(ScrollRect scrollRect, Vector2 delta) {
            yield return null;
            if(scrollRect.isActiveAndEnabled) {
                scrollRect.normalizedPosition = delta;
            }
        }
    }
}