using UnityEngine;

namespace Elarion.Extensions {
    public static class RectTransformExtensions {

        public static void CopyPosition(this RectTransform toTransform, RectTransform fromTransform, bool copySize = false) {

            toTransform.position = fromTransform.position;

            if(copySize) {
                toTransform.CopySize(fromTransform);
            }
        }

        public static void CopySize(this RectTransform toTransform, RectTransform fromTransform) {
            toTransform.sizeDelta = fromTransform.GetComponent<RectTransform>().sizeDelta * fromTransform.lossyScale.magnitude / toTransform.lossyScale.magnitude;
        }
    }
}
