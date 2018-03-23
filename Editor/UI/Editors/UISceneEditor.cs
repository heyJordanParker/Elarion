using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.UI;
using Elarion.Utility;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI.Editors {
    [CustomEditor(typeof(UIScene), true)]
    public class UISceneEditor : UnityEditor.Editor {
        
        private static readonly List<Type> HelperComponents = Utils.GetTypesWithAttribute<UISceneHelperAttribute>();

        private SerializedProperty _initialSceneProperty;
        private SerializedProperty _firstFocusedProperty;

        private List<UIScene> _allScenes;

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

            _allScenes = SceneTools.FindSceneObjectsOfType<UIScene>();
        }
        
        private void UpdateHelpers() {
            Helpers = Utils.GetComponentsDictionary(Target.gameObject, HelperComponents);
        }
        
        public override void OnInspectorGUI() {
            if(targets.Length > 1) {
                return;
            }

            if(EditorApplication.isPlaying) {
                EGUI.Readonly(() => {
                    EditorGUILayout.ObjectField("Current Scene", UIScene.CurrentScene, typeof(GameObject), true);       
                });
            } else {
                EditorGUILayout.PropertyField(_initialSceneProperty);
            }
            
            var initialScene = _allScenes.FirstOrDefault(s => s.InitialScene);

            EGUI.Readonly(() => {
                EditorGUILayout.ObjectField("Initial Scene", initialScene, typeof(GameObject), true);
            });
            
            EditorGUILayout.PropertyField(_firstFocusedProperty);
            
            
            if(Target.InitialScene && !EditorApplication.isPlaying) {
                EditorGUILayout.HelpBox("This scene will be opened when the application starts.", MessageType.Info);
            }

            GUILayout.Space(10);
            
            EGUI.Horizontal(() => {
                GUILayout.FlexibleSpace();

                if(EditorApplication.isPlaying) {
                    GUI.enabled = !Target.OpenConditions || Target.OpenConditions.CanOpen;
                    var label = Target.IsOpened ? "Close" : "Open";
                    if(GUILayout.Button(label, GUILayout.MaxWidth(180))) {
                        if(Target.IsOpened) {
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
            });

            GUILayout.Space(10);
            
            serializedObject.ApplyModifiedProperties();
        }
    }

    public partial class UIComponentEditor {


    }
}