using System;
using Elarion;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {
	public static class EUtility {

		private static InspectorDrawer _inspectorDrawer;

		public static void DrawInInspector(EditorWindow caller, Action onGUI, string title) {
			DrawInInspector(caller, onGUI, null, title);
		}

		public static void DrawInInspector(EditorWindow caller, Action onGUI, Action<Rect, GUIStyle> onPreviewGUI, string title) {
			InspectorDrawer.name = title;
			InspectorDrawer.Initialize(caller, onGUI, onPreviewGUI);
			EditorUtility.SetDirty(InspectorDrawer);
		}

		public static InspectorDrawer InspectorDrawer {
			get {
				if(_inspectorDrawer == null) _inspectorDrawer = ScriptableObject.CreateInstance<InspectorDrawer>();
				return _inspectorDrawer;
			}
		}

		[MenuItem("GameObject/Create Child %#G", false, 0)]
		public static void AddChildAtOrigin() {
			var go = new GameObject("Anchor");
			go.transform.parent = Selection.activeGameObject.transform;
			go.transform.Reset();
			go.layer = Selection.activeGameObject.layer;
			Selection.activeGameObject = go;
		}

		[MenuItem("GameObject/Create Child %#N", true, 0)]
		public static bool CheckAddChildAtOrigin() {
			return Selection.activeGameObject != null;
		}
	}
}