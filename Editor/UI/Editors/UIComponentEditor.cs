using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.UI;
using Elarion.UI.Helpers;
using Elarion.UI.Helpers.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.Editor.UI.Editors {
    // TODO a context menu to change between inherited component types (e.g. convert a UIPanel to a UIDialog without adding/removing components); maybe serialize to JSON (as the base-est class) and deserialize as the new class?

    // TODO encapsulate all helper's editors here; when this editor is enabled - hide them and render them in addition to the UIComponent fields

    [CustomEditor(typeof(UIComponent), true)]
    [CanEditMultipleObjects]
    public partial class UIComponentEditor : UnityEditor.Editor {
        private static readonly List<Type> HelperComponents = Utils.GetTypesWithAttribute<UIComponentHelperAttribute>();

        private GUIStyle _previewStyle;

        private UIAnimator _animator;

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
            
            UpdateHelpers();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

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

                var focusableTarget = Target as UIFocusableComponent;

                GUI.enabled = focusableTarget != null && focusableTarget.Focusable;

                var isFocused = focusableTarget != null && focusableTarget.IsFocused;
                
                // if current selected game object
                label = isFocused ? "Unfocus" : "Focus";
                if(GUILayout.Button(label, GUILayout.MaxWidth(180))) {
                    if(focusableTarget != null) {
                        if(focusableTarget.IsFocused) {
                            focusableTarget.Unfocus();
                        } else {
                            focusableTarget.Focus(true);
                        }
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