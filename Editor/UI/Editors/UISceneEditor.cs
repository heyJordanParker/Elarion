using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.UI;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI.Editors {
    [CustomEditor(typeof(UIScene), true)]
    public class UISceneEditor : UnityEditor.Editor {
        
        private static readonly List<Type> HelperComponents = Utils.GetTypesWithAttribute<UISceneHelperAttribute>();

        private SerializedProperty _initialSceneProperty;
        private SerializedProperty _firstFocusedProperty;

        private Dictionary<Type, Component> Helpers { get; set; }
        
        private UIScene Target {
            get { return target as UIScene; }
        }
        
        private void OnEnable() {
            if(EditorPrefs.GetBool(Consts.HideUIComponentHelpersKey)) {
                Utils.HideBuiltinHelpers(Target);
            } else {
                Utils.ShowBuiltinHelpers(Target);
            }
            
            UpdateHelpers();
            
            _initialSceneProperty = serializedObject.FindProperty("_initialScene");
            _firstFocusedProperty = serializedObject.FindProperty("_firstFocused");
        }
        
        private void UpdateHelpers() {
            Helpers = Utils.GetComponentsDictionary(Target.gameObject, HelperComponents);
        }
        
        public override void OnInspectorGUI() {
            if(targets.Length > 1) {
                return;
            }
            
            EditorGUILayout.PropertyField(_initialSceneProperty);

            if(!Target.InitialScene) {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Initial Scene", UIScene.AllScenes.FirstOrDefault(s => s.InitialScene), typeof(GameObject), true);
                GUI.enabled = true;
            } else {
                EditorGUILayout.HelpBox("This scene will be loaded when the application starts.", MessageType.Info);
            }
            
            EditorGUILayout.PropertyField(_firstFocusedProperty);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if(Application.isPlaying) {
                GUI.enabled = !Target.OpenConditions || Target.OpenConditions.CanOpen;
                var label = Target.State.IsOpened ? "Close" : "Open";
                if(GUILayout.Button(label, GUILayout.MaxWidth(180))) {
                    if(Target.State.IsOpened) {
                        Target.Close();
                    } else {
                        Target.Open();
                    }
                }
            } else {
                if(EGUI.AddComponentsButton("Add Helper Component", Target.gameObject, Helpers)) {
                    UpdateHelpers();
                }
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            serializedObject.ApplyModifiedProperties();
        }
    }

    public partial class UIComponentEditor {


    }
}