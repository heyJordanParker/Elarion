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
        
        private GUIStyle _previewStyle;

        private UIAnimator _animator;

        protected UIComponent Target {
            get {
                return target as UIComponent;
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

            if(!Application.isPlaying && Animator) {
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
            } else if(!Animator) {
                if(GUILayout.Button("Add Animator", GUILayout.MaxWidth(250))) {
                    var components = targets.OfType<UIComponent>().ToList();

                    Undo.RecordObjects(components.Select(t => t.gameObject).ToArray(), "Add UIAnimator");
                    foreach(var component in components) {
                        component.gameObject.AddComponent<UIAnimator>();
                    }
                }
            }
            
            GUILayout.FlexibleSpace();
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
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