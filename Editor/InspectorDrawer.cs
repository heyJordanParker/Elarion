using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion.Editor {
	public class InspectorDrawer : ScriptableObject {

		private Object _originalSelection;
		private Action _onGUI;
		private Action<Rect, GUIStyle> _onPreviewGUI;

		private Type _inspectorType;

		private EditorWindow _caller;

		public void Initialize(EditorWindow callerWindow, Action onGUI, Action<Rect, GUIStyle> previewGUI) {
			_originalSelection = Selection.activeObject;
			Selection.activeObject = this;
			hideFlags = HideFlags.DontSave;
			_onGUI = onGUI;
			_onPreviewGUI = previewGUI;

			_caller = callerWindow;

			if(_inspectorType == null) {
				foreach(var T in Assembly.GetAssembly(typeof(EditorWindow)).GetTypes().Where(T => T.Name == "InspectorWindow")) {
					_inspectorType = T;
					break;
				}
			}

			UpdateInspector();
		}

		private bool _destroyed;

		private void OnDisable() {
			if(_destroyed) return;
			_destroyed = true;
			Deinitialize();
			DestroyImmediate(this);
		}

		private void UpdateInspector() {
			EditorWindow.FocusWindowIfItsOpen(_inspectorType);
			EditorWindow.FocusWindowIfItsOpen(_caller.GetType());
		}

		public void Deinitialize() { Selection.activeObject = _originalSelection; }

		public void OnGUI() {
			if(_onGUI != null) _onGUI();
			if(GUI.changed) {
				_caller.Repaint();
			}
		}

		public void OnPreviewGUI(Rect rect, GUIStyle background) {
			_onPreviewGUI(rect, background);
		}

		public bool HasPreviewGUI() { return _onPreviewGUI != null; }

		

	}
}