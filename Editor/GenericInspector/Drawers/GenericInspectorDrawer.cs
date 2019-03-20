using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.GenericInspector.Drawers {
    public abstract class GenericInspectorDrawer {
        protected GenericInspector Inspector { get; }
        protected Object Target { get; }
        protected SerializedObject SerializedObject { get; }
        
        protected GenericInspectorDrawer(GenericInspector inspector, Object target, SerializedObject serializedObject) {
            Inspector = inspector;
            Target = target;
            SerializedObject = serializedObject;
        }
        
        public virtual void BeforeDrawInspector() { }

        public virtual bool CanDrawProperty(SerializedProperty property) {
            return false;
        }

        public virtual void DrawProperty(SerializedProperty property) { }
        
        public virtual void AfterDrawInspector() { }

        public virtual void OnInspectorChanged() { }
        
    }
}