using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.UI;
using Elarion.UI.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(UIComponent), true)]
    [CanEditMultipleObjects]
    public class UIComponentEditor : UnityEditor.Editor {
        
        private static readonly Type[] HelperComponents = {
            typeof(UIAnimator),
            typeof(UIConditionalVisibility),
        };

        private Dictionary<Type, Component> _helpers;
        
        private GUIStyle _previewStyle;

        private UIAnimator _animator;
        
        protected UIComponent Target {
            get {
                return target as UIComponent;
            }
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

        protected bool HasAllHelpers {
            get {
                foreach(var helperComponent in Helpers.Values) {
                    if(!helperComponent) {
                        return false;
                    } 
                }

                return true;
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

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(!Application.isPlaying && HasAllHelpers || targets.Length > 1) {
                return;
            }
            
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();

            if(Application.isPlaying) {
                var label = Target.Opened ? "Close" : "Open";
                if(GUILayout.Button(label, GUILayout.MaxWidth(180))) {
                    if(Target.Opened) {
                        Target.Close();
                    } else {
                        Target.Open();
                    }
                }

                label = Target.Focused ? "Unfocus" : "Focus";
                if(GUILayout.Button(label, GUILayout.MaxWidth(180))) {
                    if(Target.Focused) {
                        Target.Unfocus();
                        EventSystem.current.SetSelectedGameObject(null);
                    } else {
                        Target.Focus();
                        EventSystem.current.SetSelectedGameObject(Target.gameObject);
                    }
                }

                if(Target.HasAnimator) {
                    label = "Reset";
                    if(GUILayout.Button(label, GUILayout.MaxWidth(180))) {
                        Animator.ResetToSavedPropertiesGraceful();
                        EventSystem.current.SetSelectedGameObject(Target.gameObject);
                    }
                }
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
                
                var index = EditorGUILayout.Popup(0, dropdownItems.Values.ToArray(), new GUIStyle("DropDownButton"), GUILayout.MaxWidth(250));

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
            GUI.Label(rect, Target.ToString(), PreviewStyle);
        }
    }
}