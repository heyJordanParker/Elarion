using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elarion.Extensions;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {
    public static class Utils {
        private static InspectorDrawer _inspectorDrawer;

        public static void DrawInInspector(EditorWindow caller, Action onGUI, string title) {
            DrawInInspector(caller, onGUI, null, title);
        }

        public static void DrawInInspector(EditorWindow caller, Action onGUI, Action<Rect, GUIStyle> onPreviewGUI,
            string title) {
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

        public static T CreateScriptableObject<T>() where T : ScriptableObject {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var name = "New " + ObjectNames.NicifyVariableName(typeof(T).Name) + ".asset";
            var separator = Path.DirectorySeparatorChar;

            if(path == "") {
                path = "Assets" + separator + name;
            } else {
                if(Directory.Exists(path)) {
                    path += separator;
                } else {
                    path = path.Substring(0, path.Length - Path.GetFileName(path).Length);
                }

                path += name;
            }

            var savedObject = ScriptableObject.CreateInstance(typeof(T));

            var savePath = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(savedObject, savePath);
            Selection.activeObject = savedObject;
            return savedObject as T;
        }

        public static T Create<T>() where T : MonoBehaviour {
            var go = new GameObject("New " + ObjectNames.NicifyVariableName(typeof(T).Name));

            if(Selection.activeGameObject != null) {
                go.transform.SetParent(Selection.activeGameObject.transform);
                go.transform.Reset();
                go.layer = Selection.activeGameObject.layer;
            }
            var behavior = go.AddComponent<T>();

            Selection.activeGameObject = go;
            
            return behavior;
        }
    }
}