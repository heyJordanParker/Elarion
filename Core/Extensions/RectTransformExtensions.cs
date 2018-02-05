using UnityEngine;

namespace Elarion.Extensions {
    public static class RectTransformExtensions {
        public static bool IsInside(this RectTransform transform, RectTransform container) {
            var screenTransform = transform.ToScreenSpace();
            var screenContainer = container.ToScreenSpace();

            return screenContainer.Overlaps(screenTransform);
        }

        public static Rect ToScreenSpace(this RectTransform transform) {
            var size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            var rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
            rect.x -= transform.pivot.x * size.x;
            rect.y -= (1.0f - transform.pivot.y) * size.y;
            return rect;
        }

        public static void CopyPosition(this RectTransform toTransform, RectTransform fromTransform,
            bool copySize = false) {

            toTransform.position = fromTransform.position;

            if(copySize) {
                toTransform.CopySize(fromTransform);
            }
        }

        public static void CopySize(this RectTransform toTransform, RectTransform fromTransform) {
            toTransform.sizeDelta = fromTransform.GetComponent<RectTransform>().sizeDelta *
                                    fromTransform.lossyScale.magnitude / toTransform.lossyScale.magnitude;
        }

        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot) {
            var size = rectTransform.rect.size;
            var scale = rectTransform.localScale;
            var deltaPivot = rectTransform.pivot - pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x * scale.x, deltaPivot.y * size.y * scale.y);
            
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
    }
}