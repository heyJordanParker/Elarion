using UnityEditor;
using System.Diagnostics;

namespace Elarion.Editor {
    public static class EditorUpdate {

        private static EditorApplication.CallbackFunction updateFunction;

        /// <summary>
        /// Sets an action to be called every frame in the editor imitating an update method.
        /// </summary>
        /// <param name="update">Action to be called</param>
        [Conditional("UNITY_EDITOR")]
        public static void SetUpdate(EditorApplication.CallbackFunction update) {
            updateFunction = update;
            EditorApplication.update += updateFunction;
        }

        /// <summary>
        /// Removes previously set action from update function.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void CleanUpdate() {
            EditorApplication.update -= updateFunction;
        }
    }
}