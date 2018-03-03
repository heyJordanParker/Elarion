using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Extensions {
    public static class EditorExtensions {

        public static void DrawDefaultScriptField(this UnityEditor.Editor editor) {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(editor.target as MonoBehaviour), typeof(MonoScript), false);
            GUI.enabled = true;
        }
		
    }
}