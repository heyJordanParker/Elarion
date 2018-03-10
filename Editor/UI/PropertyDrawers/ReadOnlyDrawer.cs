using Elarion.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI.PropertyDrawers {
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
        private ReadOnlyAttribute Attribute {
            get { return (ReadOnlyAttribute) attribute; }
        }
        
        public override float GetPropertyHeight(SerializedProperty property,
            GUIContent label) {
            
            if(Attribute.showOnlyWhenPlaying && !EditorApplication.isPlaying) {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
            
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
            SerializedProperty property,
            GUIContent label) {
            if(Attribute.showOnlyWhenPlaying && !EditorApplication.isPlaying) {
                return;
            }
            
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
        
        
    }
}