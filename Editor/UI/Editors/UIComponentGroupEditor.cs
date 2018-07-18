using System.Collections.Generic;
using System.Linq;
using Elarion.UI;
using Elarion.Utility;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI.Editors {
    [CustomEditor(typeof(UIComponentGroup))]
    [CanEditMultipleObjects]
    public class UIComponentGroupEditor : UnityEditor.Editor {

        private bool _foldout;

        private int _otherMembers;
        private List<UIComponentGroup> _componentGroups;
        
        private UIComponentGroup Target => target as UIComponentGroup;

        private void OnEnable() {
            _componentGroups = SceneTools.FindSceneObjectsOfType<UIComponentGroup>();
            _otherMembers = _componentGroups.Count(g => g.GroupId == Target.GroupId);
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(targets.Length > 1) {
                return;
            }
            
            _foldout = EditorGUILayout.Foldout(_foldout, "Group Members (" + _otherMembers + ")", true);

            EditorGUI.indentLevel += 1;

            if(_foldout) {
                
                DrawComponentGroup(Target);
                
                foreach(var componentGroup in _componentGroups) {
                    if(componentGroup == Target || 
                       componentGroup.GroupId != Target.GroupId) {
                        continue;
                    }

                    DrawComponentGroup(componentGroup);
                }
            }
            
            EditorGUI.indentLevel -= 1;
        }

        private void DrawComponentGroup(UIComponentGroup componentGroup) {
            EGUI.Horizontal(() => {

                GUILayout.Space(10);
                
                var style = new GUIStyle(EditorStyles.label);
                var postfix = string.Empty;  

                if(componentGroup == Target) {
                    style.fontStyle = FontStyle.Bold;
                    postfix = "(this)";
                }
                
                EditorGUILayout.PrefixLabel(componentGroup.gameObject.name + postfix, style, style);
                
                EditorGUILayout.ObjectField(componentGroup.gameObject,
                    typeof(GameObject), true);

                GUILayout.Label("Auto Open");

                var autoOpen = componentGroup.Component.OpenType == UIOpenType.Auto;
                
                if(EditorGUILayout.Toggle(autoOpen) != autoOpen) {
                    if(autoOpen) {
                        componentGroup.Component.OpenType = UIOpenType.Manual;   
                    } else {
                        SetGroupToAutoOpen(componentGroup);
                    }
                }
            });
        }

        public static void SetGroupToAutoOpen(UIComponentGroup group) {
            if(!group) {
                return;
            }

            group.Component.OpenType = UIOpenType.Auto;
            
            var allGroups = SceneTools.FindSceneObjectsOfType<UIComponentGroup>();

            foreach(var otherGroup in allGroups) {
                if(otherGroup != group &&
                   otherGroup.GroupId == group.GroupId &&
                   otherGroup.Component.OpenType == UIOpenType.Auto) {
                    otherGroup.Component.OpenType = UIOpenType.Manual;
                }
                
            }
        }
        
        
        
    }
}