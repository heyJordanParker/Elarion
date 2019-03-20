using System.Collections.Generic;
using System.Reflection;
using Elarion.Attributes;
using Elarion.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion.Editor.GenericInspector.Drawers {
    // TODO implement always visible
    public class ButtonDrawer : GenericInspectorDrawer {
        private const string EditorPrefsButtonFoldout = "ButtonFoldoutVisible";

        private readonly Dictionary<string, MethodInfo> _buttons = new Dictionary<string, MethodInfo>();
        private readonly Dictionary<string, string> _labels = new Dictionary<string, string>();
        private readonly Dictionary<string, MethodInfo> _validators = new Dictionary<string, MethodInfo>();
        
        private bool _foldout;
        
        public bool Foldout {
            get => _foldout;
            set {
                EditorPrefs.SetBool(EditorPrefsButtonFoldout, value);
                _foldout = value;
            }
        }
        
        public ButtonDrawer(GenericInspector inspector, Object target, SerializedObject serializedObject) : base(inspector, target, serializedObject) {
            _foldout = EditorPrefs.GetBool(EditorPrefsButtonFoldout, true);
            
            var methods = Target.GetType().GetMethodsRecursive();

            foreach(var method in methods) {
                foreach(InspectorButtonAttribute buttonAttribute in method.GetCustomAttributes(
                    typeof(InspectorButtonAttribute), false)) {
                    if(string.IsNullOrEmpty(buttonAttribute.Title)) {
                        buttonAttribute.Title = method.Name;
                    }
                    
                    if(!_buttons.ContainsKey(buttonAttribute.Title)) {
                        _buttons.Add(buttonAttribute.Title, method);

                        if(buttonAttribute.Label != null) {
                            _labels.Add(buttonAttribute.Title, buttonAttribute.Label);
                        }
                    }
                }

                foreach(InspectorButtonValidatorAttribute validatorAttribute in method.GetCustomAttributes(
                    typeof(InspectorButtonValidatorAttribute), false)) {
                    if(string.IsNullOrEmpty(validatorAttribute.Title)) {
                        validatorAttribute.Title = method.Name;
                    }
                    
                    if(!_validators.ContainsKey(validatorAttribute.Title)) {
                        _validators.Add(validatorAttribute.Title, method);
                    }
                }
            }

            var properties = Target.GetType().GetPropertiesRecursive();

            foreach(var property in properties) {
                foreach(InspectorButtonValidatorAttribute validatorAttribute in property.GetCustomAttributes(
                    typeof(InspectorButtonValidatorAttribute), false)) {
                    if(!_validators.ContainsKey(validatorAttribute.Title)) {
                        _validators.Add(validatorAttribute.Title, property.GetMethod);
                    }
                }
            }
        }

        public override void AfterDrawInspector() {
            if(_buttons.Count == 0) return;

            EditorGUILayout.Space();
            
            GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox) {padding = new RectOffset(5, 5, 5, 5)});

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Foldout = EditorGUILayout.Foldout(Foldout, "Actions", true, new GUIStyle("Foldout") {fontStyle = FontStyle.Bold});
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            if(Foldout) {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                foreach(var button in _buttons) {
                    var buttonTitle = button.Key;
                    var buttonMethod = button.Value;

                    var wasEnabled = GUI.enabled;
                    var isEnabled = true;
                    var hasLabel = _labels.ContainsKey(buttonTitle);

                    if(_validators.ContainsKey(buttonTitle))
                        isEnabled = (bool) _validators[buttonTitle].Invoke(Target, null);

                    GUI.enabled = isEnabled;

                    GUILayout.BeginHorizontal();

                    if(hasLabel) {
                        EditorGUILayout.PrefixLabel(_labels[buttonTitle]);
                    } else {
                        GUILayout.FlexibleSpace();
                    }

                    if(GUILayout.Button(button.Key, GUILayout.MaxWidth(250)) && buttonMethod != null) {
                        buttonMethod.Invoke(Target, null);
                    }

                    if(!hasLabel) {
                        GUILayout.FlexibleSpace();
                    }

                    GUILayout.EndHorizontal();

                    GUI.enabled = wasEnabled;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            
            GUILayout.EndVertical();
        }
    }
}