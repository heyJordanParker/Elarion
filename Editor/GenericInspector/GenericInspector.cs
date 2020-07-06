using System;
using System.Collections.Generic;
using System.Reflection;
using Elarion.Attributes;
using Elarion.Editor.GenericInspector.Drawers;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Elarion.Editor.GenericInspector {
    // TODO Info Drawer - draws an infobox below the inspector similar to the ButtonsDrawer
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true, isFallback = true)]
    public class GenericInspector : UnityEditor.Editor {

        public static GenericInspector CurrentInspector { get; private set; }
        
        public static event Action DrawAfterGUI;
        
        private bool _requiresConstantRepaint;
        
        private readonly List<GenericInspectorDrawer> _drawers = new List<GenericInspectorDrawer>();
        
        protected virtual void OnEnable() {
            if(!target || !serializedObject.targetObject) {
                return;
            }

            _drawers.Add(new ButtonDrawer(this, target, serializedObject));
            _drawers.Add(new ScriptableObjectDrawer(this, target, serializedObject));
            _drawers.Add(new ReorderableListDrawer(this, target, serializedObject));

            _requiresConstantRepaint =
                serializedObject.targetObject.GetType().GetCustomAttribute<RequiresConstantRepaintAttribute>(true) != null;
        }

        public override void OnInspectorGUI() {
            CurrentInspector = this;

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            
            BeforeDrawInspector();
            
            DrawInspector(serializedObject.GetIterator());
            
            AfterDrawInspector();

            if(DrawAfterGUI != null) {
                DrawAfterGUI();
                DrawAfterGUI = null;
            }
            
            serializedObject.ApplyModifiedProperties();

            if(EditorGUI.EndChangeCheck()) {
                OnInspectorChanged();
            }
        }

        protected virtual void BeforeDrawInspector() {
            for(int i = 0; i < _drawers.Count; ++i) {
                _drawers[i].BeforeDrawInspector();
            }
        }

        protected virtual void DrawInspector(SerializedProperty iterator) {
            for(var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false) {
                DrawProperty(iterator);
            }
        }

        protected virtual void AfterDrawInspector() {
            for(int i = 0; i < _drawers.Count; ++i) {
                _drawers[i].AfterDrawInspector();
            }
        }

        protected virtual void OnInspectorChanged() {
            for(int i = 0; i < _drawers.Count; ++i) {
                _drawers[i].OnInspectorChanged();
            }
        }

        protected virtual void DrawProperty(SerializedProperty property) {
            for(var i = 0; i < _drawers.Count; ++i) {
                var inspectorDrawer = _drawers[i];
                if(!inspectorDrawer.CanDrawProperty(property)) {
                    continue;
                }

                inspectorDrawer.DrawProperty(property);
                return;
            }

            // Default inspector behavior
            using(new EditorGUI.DisabledScope("m_Script" == property.propertyPath)) {
                EditorGUILayout.PropertyField(property, true);
            }
        }
        
        public override bool RequiresConstantRepaint() {
            return _requiresConstantRepaint;
        }
    }
}