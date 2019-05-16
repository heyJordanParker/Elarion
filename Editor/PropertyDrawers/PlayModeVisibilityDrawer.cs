using Elarion.Common.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(PlayModeVisibilityAttribute))]
    public class PlayModeVisibilityDrawer : PropertyDrawer {
        private PlayModeVisibilityAttribute Attribute => (PlayModeVisibilityAttribute) attribute;

        private bool IsVisible => Attribute.DrawInPlayMode == EditorApplication.isPlaying;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            // If the target field passes the check or we're just disabling the field - return the actual height
            if(IsVisible) {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            // Invisible property; Remove the default spacing
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            
            var guiEnabled = GUI.enabled;
            GUI.enabled = Attribute.LockInPlayMode && EditorApplication.isPlaying || Attribute.LockInEditorMode && !EditorApplication.isPlaying;
            
            if(IsVisible) {
                EditorGUI.PropertyField(position, property, label, true);
            }

            GUI.enabled = guiEnabled;
        }
    }
}