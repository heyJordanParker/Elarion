using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Extensions {
    public static class MonoBehaviourExtensions {
        public static bool IsPrefab(this MonoBehaviour behaviour) {
            return PrefabUtility.IsPartOfAnyPrefab(behaviour);
        }
    }
}