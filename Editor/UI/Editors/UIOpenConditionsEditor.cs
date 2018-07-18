using System.Text;
using Elarion.UI.Helpers;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI.Editors {
    [CustomEditor(typeof(UIOpenConditions), true)]
    public class UIOpenConditionsEditor : UnityEditor.Editor {

        protected SerializedProperty platformProperty;
        protected SerializedProperty screenWidthProperty;
        protected SerializedProperty screenHeightProperty;
        protected SerializedProperty parentStateProperty;
        protected SerializedProperty screenOrientationProperty;
        
        protected UIOpenConditions Target {
            get { return target as UIOpenConditions; }
        }

        private void OnEnable() {
            platformProperty = serializedObject.FindProperty("platform");
            screenWidthProperty = serializedObject.FindProperty("screenWidth");
            screenHeightProperty = serializedObject.FindProperty("screenHeight");
            parentStateProperty = serializedObject.FindProperty("state");
            screenOrientationProperty = serializedObject.FindProperty("orientation");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            var boldLabel = new GUIStyle("BoldLabel");

            EditorGUILayout.LabelField("Opens only on the specified platforms", boldLabel);
            
            Target.platformCondition = EditorGUILayout.Toggle("Platform Condition", Target.platformCondition);

            if(Target.platformCondition) {
                EditorGUILayout.PropertyField(platformProperty, new GUIContent("Show on Platforms"));
            }
            
            EditorGUILayout.LabelField("Opens only on some screen sizes", boldLabel);
            
            Target.screenSizeCondition = EditorGUILayout.Toggle("Screen Size Condition", Target.screenSizeCondition);

            if(Target.screenSizeCondition) {
                EditorGUILayout.PropertyField(screenWidthProperty, new GUIContent("Min/Max Screen Width"));
                EditorGUILayout.PropertyField(screenHeightProperty, new GUIContent("Min/Max Screen Height"));
            }
            
            EditorGUILayout.LabelField("Opens only in some states", boldLabel);
            
            Target.stateCondition = EditorGUILayout.Toggle("State Condition", Target.stateCondition);

            if(Target.stateCondition) {
                EditorGUILayout.PropertyField(parentStateProperty, new GUIContent("Show in State"));
            }
            
            EditorGUILayout.LabelField("Opens only on the specified screen orientation", boldLabel);
            
            Target.orientationCondition = EditorGUILayout.Toggle("Orientation Condition", Target.orientationCondition);

            if(Target.orientationCondition) {
                EditorGUILayout.PropertyField(screenOrientationProperty, new GUIContent("Show on Screen Orientation"));
            }

            if(Application.isPlaying) {
                var text = new StringBuilder().AppendFormat("With current configuration {0} ", Target.name);
                text.Append(Target.CanOpen ? "would open." : "would not open.");
                EditorGUILayout.HelpBox(text.ToString(), MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}