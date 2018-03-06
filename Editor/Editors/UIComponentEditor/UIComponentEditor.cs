using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.Extensions;
using Elarion.UI;
using Elarion.UI.Animation;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(UIComponent), true)]
    [CanEditMultipleObjects]
    public partial class UIComponentEditor : UnityEditor.Editor {

        // TODO dynamically load those based on an interface/inheritance 
        private static readonly Type[] HelperComponents = {
            typeof(UIAnimator),
            typeof(UIOpenConditions),
            typeof(UIEffect),
            typeof(UIResizable),
            typeof(UIDraggable),
            typeof(UISubmitHandler),
            typeof(UICancelHandler),
        };

        private Dictionary<Type, Component> _helpers;

        private GUIStyle _previewStyle;

        private UIAnimator _animator;

        protected UIComponent Target {
            get { return target as UIComponent; }
        }

        protected Dictionary<Type, Component> Helpers {
            get {
                if(_helpers == null) {
                    _helpers = new Dictionary<Type, Component>();
                    UpdateHelpers();
                }

                return _helpers;
            }
        }

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
                Utils.HideHelpers(Target);
            } else {
                Utils.ShowHelpers(Target);
            }
        }

        public override void OnInspectorGUI() {
//            if(Target is UIScene) {
//                DrawSceneInspectorGUI();
//                return;
//            }

            DrawComponentInspectorGUI();
        }

        private void DrawComponentInspectorGUI() {
            base.OnInspectorGUI();

            DrawHelpersGUI();
        }

        private void DrawHelpersGUI() {
            var hasAllHelpers = Helpers.Values.All(h => h);
                
            if(!Application.isPlaying && hasAllHelpers || targets.Length > 1) {
                return;
            }

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

                GUI.enabled = Target.Focusable;

                UIComponent tempQualifier = Target;
                label = tempQualifier.State.IsFocusedThis || tempQualifier.State.IsFocusedChild ? "Unfocus" : "Focus";
                if(GUILayout.Button(label, GUILayout.MaxWidth(180))) {
                    UIComponent tempQualifier1 = Target;
                    if(tempQualifier1.State.IsFocusedThis || tempQualifier1.State.IsFocusedChild) {
                        Target.Unfocus();
                    } else {
                        Target.Focus(true);
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
                var dropdownItems = new Dictionary<Type, string>();

                dropdownItems.Add(typeof(int), "Add Helper Component");

                foreach(var helper in Helpers) {
                    if(helper.Value != null) {
                        // No need to add existing components
                        continue;
                    }

                    dropdownItems.Add(helper.Key, ObjectNames.NicifyVariableName(helper.Key.Name.Replace("UI", "")));
                }

                var index = EditorGUILayout.Popup(0, dropdownItems.Values.ToArray(), new GUIStyle("DropDownButton"),
                    GUILayout.MaxWidth(250));

                if(index != 0) {
                    var component = dropdownItems.ElementAt(index).Key;

                    Undo.RecordObject(target, "Add " + component.Name);

                    Target.gameObject.AddComponent(component);

                    UpdateHelpers();
                }
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        private void UpdateHelpers() {
            Helpers.Clear();
            foreach(var helperComponent in HelperComponents) {
                Helpers.Add(helperComponent, Target.GetComponent(helperComponent));
            }
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