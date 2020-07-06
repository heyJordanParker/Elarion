using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI {
    public static class EGUI {
        public static readonly GUIStyle DropdownButton = new GUIStyle("DropDownButton");

        public static void Horizontal(Action horizontalLayout) {
            GUILayout.BeginHorizontal();
            horizontalLayout();
            GUILayout.EndHorizontal();
        }

        public static void Vertical(Action verticalLayout) {
            GUILayout.BeginVertical();
            verticalLayout();
            GUILayout.EndVertical();
        }

        public static void Readonly(Action readonlyGUI) {
            var guiEnabled = GUI.enabled;

            GUI.enabled = false;
            readonlyGUI();
            GUI.enabled = guiEnabled;
        }

        public static bool AddComponentsButton(string title, GameObject target,
            Dictionary<Type, Component> components) {
            var dropdownItems = new Dictionary<Type, string> {{typeof(int), title}};

            foreach(var helper in components) {
                if(helper.Value != null) {
                    // No need to add existing components
                    continue;
                }

                dropdownItems.Add(helper.Key, ObjectNames.NicifyVariableName(helper.Key.Name.Replace("UI", "")));
            }

            var index = EditorGUILayout.Popup(0, dropdownItems.Values.ToArray(), DropdownButton,
                GUILayout.MaxWidth(250));

            if(index != 0) {
                var component = dropdownItems.ElementAt(index).Key;

                Undo.RecordObject(target, "Add " + component.Name);

                target.AddComponent(component);
                return true;
            }

            return false;
        }
    }
}