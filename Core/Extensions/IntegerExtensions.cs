using UnityEngine;

namespace Elarion.Extensions {
    public static class IntegerExtensions {
		 
        public static float Abs(this int value) {
            return Mathf.Abs(value);
        }

        public static float Sign(this int value) {
            return Mathf.Sign(value);
        }
    }
}