namespace Elarion.Editor {
	using UnityEditor;
	using UnityEngine;

	public class InspectorWindow : EditorWindow {

		[MenuItem("Window/Test")]
		public static void Test() {
			GetWindow<InspectorWindow>("Test");
		}

		GameObject activeGO;
		Editor editor;

		void Update() {
			if(activeGO == Selection.activeGameObject)
				return;

			activeGO = Selection.activeGameObject;
			editor = null;

			if(activeGO != null)
				editor = Editor.CreateEditor(activeGO.transform);

			Repaint();
		}

		void OnGUI() {
			GameObject go = Selection.activeGameObject;
			if(go == null)
				return;

			// Use the registered editor for `Transform`
			EditorGUILayout.InspectorTitlebar(true, editor.target);
			editor.OnInspectorGUI();
		}

	}
}