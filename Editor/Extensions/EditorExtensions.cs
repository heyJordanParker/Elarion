using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Extensions {
    public static class EditorExtensions {
        public static void DrawDefaultScriptField(this UnityEditor.Editor editor) {
            GUI.enabled = false;
            var monoScript = editor.target.GetType().IsSubclassOf(typeof(ScriptableObject))
                ? MonoScript.FromScriptableObject(editor.target as ScriptableObject)
                : MonoScript.FromMonoBehaviour(editor.target as MonoBehaviour);
            EditorGUILayout.ObjectField("Script",
                monoScript, typeof(MonoScript), false);
            GUI.enabled = true;
        }
    }
}