using UnityEngine;
using UnityEngine.UI;

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

        /// <summary>
        /// Finds the closest selectable left to the current one. Prioritizes selectables that share a common parent with the starting one.
        /// </summary>
        /// <param name="toTransform">Starting transform. The search is relative to it.</param>
        /// <returns>The closest selectable or null if none is found.</returns>
        public static Selectable FindCloseSelectableOnLeft(this RectTransform toTransform) {
            return toTransform.FindCloseSelectable(toTransform.rotation * Vector3.left);
        }

        /// <summary>
        /// Finds the closest selectable right to the current one. Prioritizes selectables that share a common parent with the starting one.
        /// </summary>
        /// <param name="toTransform">Starting transform. The search is relative to it.</param>
        /// <returns>The closest selectable or null if none is found.</returns>
        public static Selectable FindCloseSelectableOnRight(this RectTransform toTransform) {
            return toTransform.FindCloseSelectable(toTransform.rotation * Vector3.right);
        }

        /// <summary>
        /// Finds the closest selectable up to the current one. Prioritizes selectables that share a common parent with the starting one.
        /// </summary>
        /// <param name="toTransform">Starting transform. The search is relative to it.</param>
        /// <returns>The closest selectable or null if none is found.</returns>
        public static Selectable FindCloseSelectableOnUp(this RectTransform toTransform) {
            return toTransform.FindCloseSelectable(toTransform.rotation * Vector3.up);
        }

        /// <summary>
        /// Finds the closest selectable down to the current one. Prioritizes selectables that share a common parent with the starting one.
        /// </summary>
        /// <param name="toTransform">Starting transform. The search is relative to it.</param>
        /// <returns>The closest selectable or null if none is found.</returns>
        public static Selectable FindCloseSelectableOnDown(this RectTransform toTransform) {
            return toTransform.FindCloseSelectable(toTransform.rotation * Vector3.down);
        }

        /// <summary>
        /// Finds the closest selectable in a direction. Prioritizes selectables that share a common parent with the starting one.
        /// </summary>
        /// <param name="toTransform">Starting transform. The search is relative to it.</param>
        /// <param name="dir">The Direction of the search.</param>
        /// <returns>The closest selectable or null if none is found.</returns>
        public static Selectable FindCloseSelectable(this RectTransform toTransform, Vector3 dir) {
            dir = dir.normalized;

            var pointOnEdge = toTransform.TransformPoint(GetPointOnRectEdge(
                toTransform,
                Quaternion.Inverse(toTransform.rotation) * dir));

            var maxCorrelation = float.NegativeInfinity;
            var minParentDistance = int.MaxValue;

            Selectable result = null;

            var selectables = Selectable.allSelectables.ToArray();

            foreach(var selectable in selectables) {
                if(selectable == null ||
                   selectable.gameObject == toTransform.gameObject ||
                   !selectable.IsInteractable() ||
                   selectable.navigation.mode == Navigation.Mode.None) {
                    continue;
                }

                var transform = selectable.transform as RectTransform;

                if(transform == null) {
                    continue;
                }

                var position = (Vector3) transform.rect.center;

                var distance = selectable.transform.TransformPoint(position) - pointOnEdge;

                var correlation = Vector3.Dot(dir, distance);

                if(correlation <= 0.0) {
                    // the selectable isn't in the direction we need
                    continue;
                }

                var normalizedCorrelation = correlation / distance.sqrMagnitude;

                var parentDistance = CommonParentDistance(transform, toTransform);

                if(parentDistance < minParentDistance) {
                    // this selectable is in the direction we're looking into and the closest to the original one in the hierarchy
                    minParentDistance = parentDistance;
                    result = selectable;
                    maxCorrelation = normalizedCorrelation;
                    continue;
                }

                if(parentDistance == minParentDistance) {
                    // from all the selectables with minimal hierarchical difference find the one that correlates best with our direction
                    if(normalizedCorrelation > (double) maxCorrelation) {
                        // this selecgtable is 
                        maxCorrelation = normalizedCorrelation;
                        result = selectable;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the distance to a common parent between two transforms.
        /// One parent distance = one hop up in the hierarchy.
        /// The hops continue until we the object found is a common parent to both transforms.
        /// </summary>
        /// <param name="fromTransform">The transform from which we're calculating the distancek</param>
        /// <param name="toTransform">The second transform</param>
        /// <returns>
        /// The common parent distance.
        /// Returns 0 if one transform is child of the other.
        /// Returns -1 if no common parent is found.
        /// </returns>
        public static int CommonParentDistance(this Transform fromTransform, Transform toTransform) {
            if(fromTransform.IsChildOf(toTransform)) {
                return 0;
            }

            var parent = fromTransform;

            var currentDistance = 0;

            while(parent != null) {
                if(parent.IsParentOf(toTransform)) {
                    return currentDistance;
                }

                ++currentDistance;
                parent = parent.parent;
            }

            return -1;
        }

        private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir) {
            if(rect == null)
                return Vector3.zero;
            if(dir != Vector2.zero)
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
            return dir;
        }
    }
}