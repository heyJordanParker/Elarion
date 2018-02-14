using Elarion.UI;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(UIComponent), true)]
    [CanEditMultipleObjects]
    public class UIComponentEditor : UnityEditor.Editor {
        
        private GUIStyle _previewStyle;

        protected UIComponent Target {
            get {
                return target as UIComponent;
            }
        }

        protected GUIStyle PreviewStyle {
            get {
                if(_previewStyle == null) {
                    _previewStyle = new GUIStyle("WhiteLabel") {
                        richText = true,
                        alignment = TextAnchor.UpperLeft,
                        fontStyle = FontStyle.Normal
                    };
                }

                return _previewStyle;
            }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            
            // TODO two columns for those
            
            var label = Target.Opened ? "Close" : "Open";
            if(GUILayout.Button(label)) {
                if(Target.Opened) {
                    Target.Close();
                } else {
                    Target.Open();
                }
            }

            label = Target.Focused ? "Unfocus" : "Focus";
            if(GUILayout.Button(label)) {
                if(Target.Focused) {
                    Target.Unfocus();
                } else {
                    Target.Focus();
                }
            }
            
            EditorGUI.EndDisabledGroup();
            
            // record undo
            // Add Animator
        }

        public override bool HasPreviewGUI() {
            return true;

        }

        public override bool RequiresConstantRepaint() {
            return true;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background) {
            GUI.Label(rect, Target.ToString(), PreviewStyle);
        }
    }
}