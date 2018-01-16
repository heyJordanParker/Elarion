using Elarion.UI.Animation;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Elarion.Editor.Editors {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIAnimator))]
    public class UIAnimatorEditor : UnityEditor.Editor {
        private ReorderableList _animations;
        private int _selectedIndex = -1;

        private void OnEnable() {
            _animations = new ReorderableList(serializedObject,
                serializedObject.FindProperty("_animations"),
                true, true, true, true) {
                drawHeaderCallback = rect => {
                    EditorGUI.LabelField(rect, "Animations", EditorStyles.boldLabel);
                },
                onSelectCallback = list => {
                    var animation =
                        list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("animation").objectReferenceValue as
                            ScriptableObject;
                    _selectedIndex = list.index;

                    if(animation)
                        EditorGUIUtility.PingObject(animation);
                },
                drawElementCallback =
                    (rect, index, isActive, isFocused) => {
                        var element = _animations.serializedProperty.GetArrayElementAtIndex(index);
                        rect.y += 2;


                        var dropdownPosition = new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight);
                        var animationPosition = new Rect(rect.x + 160, rect.y, rect.width - 160, EditorGUIUtility.singleLineHeight);

                        if(_selectedIndex == index) {
                            EditorGUI.LabelField(dropdownPosition, "Trigger", EditorStyles.whiteBoldLabel);
                            EditorGUI.LabelField(animationPosition, "Animation", EditorStyles.whiteBoldLabel);

                            dropdownPosition.y += EditorGUIUtility.singleLineHeight + 2;
                            animationPosition.y += EditorGUIUtility.singleLineHeight + 2;
                        }
                        
                        EditorGUI.PropertyField(dropdownPosition, element.FindPropertyRelative("type"), GUIContent.none);
                        EditorGUI.PropertyField(animationPosition, element.FindPropertyRelative("animation"), GUIContent.none);
                    },
                elementHeightCallback = index => _selectedIndex == index ? 42 : 21,
                onChangedCallback = list => { _selectedIndex = list.index; }
            };
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            _animations.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}