using Elarion.UI;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(UIConditionalVisibility), true)]
    public class UIConditionalVisibilityEditor : UnityEditor.Editor {

        protected SerializedProperty platformProperty;
        protected SerializedProperty screenWidthProperty;
        protected SerializedProperty screenHeightProperty;
        protected SerializedProperty parentWidthProperty;
        protected SerializedProperty parentHeightProperty;
        protected SerializedProperty parentStateProperty;
        protected SerializedProperty screenOrientationProperty;
        
        protected UIConditionalVisibility Target {
            get { return target as UIConditionalVisibility; }
        }

        private void OnEnable() {
            platformProperty = serializedObject.FindProperty("platform");
            screenWidthProperty = serializedObject.FindProperty("screenWidth");
            screenHeightProperty = serializedObject.FindProperty("screenHeight");
            parentWidthProperty = serializedObject.FindProperty("parentWidth");
            parentHeightProperty = serializedObject.FindProperty("parentHeight");
            parentStateProperty = serializedObject.FindProperty("parentState");
            screenOrientationProperty = serializedObject.FindProperty("orientation");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            var boldLabel = new GUIStyle("BoldLabel");

            EditorGUILayout.LabelField("Visible only on the specified platforms", boldLabel);
            
            Target.platformFilter = EditorGUILayout.Toggle("Platform Filter", Target.platformFilter);

            if(Target.platformFilter) {
                EditorGUILayout.PropertyField(platformProperty, new GUIContent("Show on Platforms"));
            }
            
            EditorGUILayout.LabelField("Visible only on some screen sizes", boldLabel);
            
            Target.screenSizeFilter = EditorGUILayout.Toggle("Screen Size Filter", Target.screenSizeFilter);

            if(Target.screenSizeFilter) {
                EditorGUILayout.PropertyField(screenWidthProperty, new GUIContent("Min/Max Screen Width"));
                EditorGUILayout.PropertyField(screenHeightProperty, new GUIContent("Min/Max Screen Height"));
            }
            
            EditorGUILayout.LabelField("Visible only on some parent sizes", boldLabel);
            
            Target.parentSizeFilter = EditorGUILayout.Toggle("Parent Size Filter", Target.parentSizeFilter);

            if(Target.parentSizeFilter) {
                EditorGUILayout.PropertyField(parentWidthProperty, new GUIContent("Min/Max Parent Width"));
                EditorGUILayout.PropertyField(parentHeightProperty, new GUIContent("Min/Max Parent Height"));
            }
            
            EditorGUILayout.LabelField("Visible only in some parent states", boldLabel);
            
            Target.parentStateFilter = EditorGUILayout.Toggle("Parent State Filter", Target.parentStateFilter);

            if(Target.parentStateFilter) {
                EditorGUILayout.PropertyField(parentStateProperty, new GUIContent("Show in Parent State"));
            }
            
            EditorGUILayout.LabelField("Visible only on the specified screen orientation (Mobile Only)", boldLabel);
            
            Target.orientationFilter = EditorGUILayout.Toggle("Orientation Filter", Target.orientationFilter);

            if(Target.orientationFilter) {
                EditorGUILayout.PropertyField(screenOrientationProperty, new GUIContent("Show on Screen Orientation"));
            }

            EditorGUILayout.HelpBox("This will be visible when...", MessageType.Info, true);

            if(GUILayout.Button("Test")) {
                Debug.Log(Screen.orientation);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}