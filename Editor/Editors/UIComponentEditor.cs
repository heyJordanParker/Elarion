using System;
using System.Collections.Generic;
using Elarion.Attributes;
using Elarion.Editor.UI;
using Elarion.UI;
using Elarion.UI.Helpers.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.Editor.Editors {
    // TODO a context menu to change between inherited component types (e.g. convert a UIPanel to a UIDialog without adding/removing components); maybe serialize to JSON (as the base-est class) and deserialize as the new class?

    // TODO encapsulate all helper's editors here; when this editor is enabled - hide them and render them in addition to the UIComponent fields

    [CustomEditor(typeof(UIComponent), true)]
    [CanEditMultipleObjects]
    public partial class UIComponentEditor : UnityEditor.Editor {
        private static readonly List<Type> HelperComponents = Utils.GetTypesWithAttribute<UIComponentHelperAttribute>();

        private GUIStyle _previewStyle;

        private UIAnimator _animator;
        private SerializedProperty _openType;
        private SerializedProperty _closeType;
        
        private SerializedProperty _beforeOpenEvent;
        private SerializedProperty _afterOpenEvent;
        
        private SerializedProperty _beforeCloseEvent;
        private SerializedProperty _afterCloseEvent;
        private SerializedProperty _overrideParent;

        protected UIComponent Target {
            get { return target as UIComponent; }
        }

        protected Dictionary<Type, Component> Helpers { get; set; }

        protected UIAnimator Animator {
            get {
                if(_animator == null) {
                    _animator = Target.GetComponent<UIAnimator>();
                }

                return _animator;
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

        private void OnEnable() {
            if(EditorPrefs.GetBool(Consts.HideUIComponentHelpersKey)) {
                Utils.HideBuiltinHelpers(Target);
            } else {
                Utils.ShowBuiltinHelpers(Target);
            }
            
            _openType = serializedObject.FindProperty("_openType");
            _closeType = serializedObject.FindProperty("_closeType");
            
            _overrideParent = serializedObject.FindProperty("_overrideParentComponent");
            
            _beforeOpenEvent = serializedObject.FindProperty("_beforeOpenEvent");
            _afterOpenEvent = serializedObject.FindProperty("_afterOpenEvent");
            _beforeCloseEvent = serializedObject.FindProperty("_beforeCloseEvent");
            _afterCloseEvent = serializedObject.FindProperty("_afterCloseEvent");
            
            UpdateHelpers();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(Target.ParentComponent) {
                EditorGUILayout.PropertyField(_openType, new GUIContent("Open Type"));
                EditorGUILayout.PropertyField(_closeType, new GUIContent("Close Type"));
            } else {
                var toggle = EditorGUILayout.Toggle(new GUIContent("Auto Open"), Target.OpenType == UIOpenType.Auto);

                if(toggle) {
                    _openType.intValue = (int) UIOpenType.Auto;
                    var group = Target.GetComponent<UIComponentGroup>();
                    if(group) {
                        UIComponentGroupEditor.SetGroupToAutoOpen(group);
                    }
                } else {
                    _openType.intValue = (int) UIOpenType.Manual;
                }
                
                EGUI.Readonly(() => {
                    EditorGUILayout.Toggle(new GUIContent("Manual Close"), true);
                    
                    _closeType.intValue = (int) UIOpenType.Manual;
                });
            }
            
            EditorGUILayout.PropertyField(_overrideParent, new GUIContent("Override Parent"));
            
            GUILayout.Space(10);
            
            EditorGUILayout.PropertyField(_beforeOpenEvent);
            EditorGUILayout.PropertyField(_afterOpenEvent);
            
            EditorGUILayout.PropertyField(_beforeCloseEvent);
            EditorGUILayout.PropertyField(_afterCloseEvent);

            if(targets.Length > 1) {
                return;
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if(Application.isPlaying) {
                GUI.enabled = !Target.OpenConditions || Target.OpenConditions.CanOpen;
                var label = Target.IsOpened ? "Close" : "Open";
                if(GUILayout.Button(label, GUILayout.MaxWidth(180))) {
                    if(Target.IsOpened) {
                        Target.Close();
                    } else {
                        Target.Open();
                    }
                }

                GUI.enabled = Target.HasAnimator;

                label = "Reset";
                if(GUILayout.Button(label, GUILayout.MaxWidth(180))) {
                    Animator.ResetToSavedPropertiesGraceful();
                    EventSystem.current.SetSelectedGameObject(Target.gameObject);
                }

                GUI.enabled = true;
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

        private void UpdateHelpers() {
            Helpers = Utils.GetComponentsDictionary(Target.gameObject, HelperComponents);
        }

        public override bool HasPreviewGUI() {
            return true;

        }

        public override bool RequiresConstantRepaint() {
            return true;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background) {
            GUI.Label(rect, Target.Description, PreviewStyle);
        }
    }
}