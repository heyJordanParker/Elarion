using UnityEngine;

namespace Elarion.Extensions {
    public static class VectorExtensions {

        public static Vector2 EaseTo(this Vector2 start, Vector2 end, float value, Ease easeType = Ease.Linear) {
            return new Vector2(start.x.EaseTo(end.x, value, easeType), start.y.EaseTo(end.y, value, easeType));
        }

        public static Vector3 EaseTo(this Vector3 start, Vector3 end, float value, Ease easeType = Ease.Linear) {
            return new Vector3(start.x.EaseTo(end.x, value, easeType), start.y.EaseTo(end.y, value, easeType), 
                               start.z.EaseTo(end.z, value, easeType));
        }

        public static float DistanceXZ(this Vector3 x, Vector3 other) { return Vector2.Distance(new Vector2(x.x, x.z), new Vector2(other.x, other.z)); }

        public static Vector4 EaseTo(this Vector4 start, Vector4 end, float value, Ease easeType = Ease.Linear) {
            return new Vector4(start.x.EaseTo(end.x, value, easeType), start.y.EaseTo(end.y, value, easeType),
                               start.z.EaseTo(end.z, value, easeType), start.w.EaseTo(end.w, value, easeType));
        }

        public static bool BetweenPoints(this Vector2 v, Vector2 fromPoint, Vector2 toPoint) {
            var minVector = new Vector2(
                Mathf.Min(fromPoint.x, toPoint.x),
                Mathf.Min(fromPoint.y, toPoint.y)
            );
            var maxVector = new Vector2(
                Mathf.Max(fromPoint.x, toPoint.x),
                Mathf.Max(fromPoint.y, toPoint.y)
            );

            return (v.x >= minVector.x) && (v.x <= maxVector.x) &&
                   (v.y >= minVector.y) && (v.y <= maxVector.y);
        }

        public static bool BetweenPoints(this Vector3 v, Vector3 fromPoint, Vector3 toPoint) {
            var minVector = new Vector3(
                Mathf.Min(fromPoint.x, toPoint.x),
                Mathf.Min(fromPoint.y, toPoint.y),
                Mathf.Min(fromPoint.z, toPoint.z)
            );
            var maxVector = new Vector3(
                Mathf.Max(fromPoint.x, toPoint.x),
                Mathf.Max(fromPoint.y, toPoint.y),
                Mathf.Max(fromPoint.z, toPoint.z)
            );

            return (v.x >= minVector.x) && (v.x <= maxVector.x) &&
                   (v.y >= minVector.y) && (v.y <= maxVector.y) &&
                   (v.z >= minVector.z) && (v.z <= maxVector.z);
        }

    }
}