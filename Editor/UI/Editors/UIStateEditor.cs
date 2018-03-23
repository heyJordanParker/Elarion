using Elarion.UI;
using UnityEditor;

namespace Elarion.Editor.UI.Editors {
    [CustomEditor(typeof(UIState))]
    public class UIStateEditor : UnityEditor.Editor {
        
        private UIState Target {
            get { return target as UIState; }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EGUI.Vertical(() => {
                EGUI.Horizontal(() => {
                    EditorGUILayout.Toggle("Opened", Target.IsOpened);
                    EditorGUILayout.Toggle("In Transition", Target.IsInTransition);
                });
                EGUI.Horizontal(() => {
                    EditorGUILayout.Toggle("Disabled", Target.IsDisabled);
                    EditorGUILayout.Toggle("Interactable", Target.IsInteractable);    
                });
                EGUI.Horizontal(() => {
                    EditorGUILayout.Toggle("Focused", Target.IsFocusedThis);
                });
            });
        }

        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}