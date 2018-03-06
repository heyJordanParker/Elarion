using Elarion.UI;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(UIState))]
    public class UIStateEditor : UnityEditor.Editor {
        
        private UIState Target {
            get { return target as UIState; }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            GUILayout.BeginVertical();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.Toggle("Opened", Target.IsOpened);
            EditorGUILayout.Toggle("In Transition", Target.IsInTransition);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.Toggle("Disabled", Target.IsDisabled);
            EditorGUILayout.Toggle("Interactable", Target.IsInteractable);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.Toggle("Rendering", Target.IsRendering);
            EditorGUILayout.Toggle("Focused", Target.IsFocusedThis);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.Toggle("Rendering Child", Target.IsRenderingChild);
            EditorGUILayout.Toggle("Focused Child", Target.IsFocusedChild);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}