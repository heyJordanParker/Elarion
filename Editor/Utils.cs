using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Elarion;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {
    public static class Utils {

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

        public static void DisplayAsyncProgressBar(string loadingText, float progress) {
            AsyncProgressBar.Display(loadingText, progress);
        }

        public static void ClearProgressBar() {
            AsyncProgressBar.Clear();
        }

        public static AddToBuildSettingsResult AddSceneToBuildSettings(string newScenePath) {
            if(Path.GetExtension(newScenePath) != ".unity") {
                return AddToBuildSettingsResult.InvalidScene;
            }
            var currentScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if(currentScenes.Any(scene => scene.path.Equals(newScenePath))) {
                return AddToBuildSettingsResult.DuplicateScene;
            }
            currentScenes.Add(new EditorBuildSettingsScene(newScenePath, true));
            EditorBuildSettings.scenes = currentScenes.ToArray();
            return AddToBuildSettingsResult.Successful;
        }
    }
}