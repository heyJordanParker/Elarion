using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Inspectors {
	[CustomEditor(typeof(InspectorDrawer))]
	public class InspectorDrawerInspector : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			Target.OnGUI();
			if(Event.current.keyCode == KeyCode.Escape) Target.Deinitialize();
			if(GUI.changed) Repaint();
		}

		public override void OnPreviewGUI(Rect rect, GUIStyle background) {
			Target.OnPreviewGUI(rect, background);
		}

		public override bool HasPreviewGUI() { return Target.HasPreviewGUI(); }

		InspectorDrawer Target { get { return target as InspectorDrawer; } }
	}
}