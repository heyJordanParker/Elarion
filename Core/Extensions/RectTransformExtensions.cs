using UnityEngine;

namespace Elarion.Extensions {
    public static class RectTransformExtensions {

        public static void CopyPosition(this RectTransform toTransform, RectTransform fromTransform, Canvas toCanvas = null,
            Canvas fromCanvas = null, bool copySize = false) {

            if(toCanvas == null) {
                toCanvas = toTransform.root.GetComponentInChildren<Canvas>();
            }

            if(fromCanvas == null) {
                fromCanvas = fromTransform.root.GetComponentInChildren<Canvas>();
            }

            Vector2 screenSpacePosition;

            // Get the transform position in screenSpace
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fromTransform, Vector2.zero, null, out screenSpacePosition);

            // Invert to get the correct position
            screenSpacePosition = -screenSpacePosition;


            var fromCanvasRect = fromCanvas.GetComponent<RectTransform>().rect;

            screenSpacePosition = new Vector2(screenSpacePosition.x * (Screen.width / fromCanvasRect.width), screenSpacePosition.y * (Screen.height / fromCanvasRect.height));

            var toCanvasPosition = toCanvas.GetComponent<RectTransform>().InverseTransformPoint(screenSpacePosition);

            toTransform.anchoredPosition = toCanvasPosition;

            if(copySize) {
                toTransform.CopySize(fromTransform, fromCanvas, toCanvas);
            }
        }

        public static void CopySize(this RectTransform toTransform, RectTransform fromTransform,
            Canvas fromCanvas = null, Canvas toCanvas = null) {
            if(toCanvas == null) {
                toCanvas = toTransform.root.GetComponentInChildren<Canvas>();
            }

            if(fromCanvas == null) {
                fromCanvas = fromTransform.root.GetComponentInChildren<Canvas>();
            }

            toTransform.sizeDelta = fromTransform.GetComponent<RectTransform>().sizeDelta * (fromCanvas.scaleFactor / toCanvas.scaleFactor);
        }
    }
}
