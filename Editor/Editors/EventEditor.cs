using Elarion.Workflows.Events;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(SimpleEvent))]
    public class EventEditor : UnityEditor.Editor {

        private SimpleEvent Target {
            get { return target as SimpleEvent; }
        }
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUI.enabled = EditorApplication.isPlaying;

            if(GUILayout.Button("Raise"))
                Target.Raise();
        }
    }
}