using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elarion.Extensions;
using Elarion.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Editor {
    internal static class Utils {

        public static void ShowHelpers(UIComponent component) {
            component.Renderer.hideFlags = HideFlags.None;
            component.GetComponent<GraphicRaycaster>().hideFlags = HideFlags.None;
            if(component.HasComponent<CanvasRenderer>()) {
                component.GetComponent<CanvasRenderer>().hideFlags = HideFlags.None;
            }
            
            var scene = component as UIScene;

            if(scene) {
                scene.CanvasGroup.hideFlags = HideFlags.None;
            }
        }
        
        public static void HideHelpers(UIComponent component) {
            component.Renderer.hideFlags = HideFlags.HideInInspector;
            component.GetComponent<GraphicRaycaster>().hideFlags = HideFlags.HideInInspector;
            if(component.HasComponent<CanvasRenderer>()) {
                component.GetComponent<CanvasRenderer>().hideFlags = HideFlags.HideInInspector;
            }

            var scene = component as UIScene;

            if(scene) {
                scene.CanvasGroup.hideFlags = HideFlags.HideInInspector;
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
            
            Undo.RegisterCreatedObjectUndo(savedObject, "Creating " + name);
            
            return savedObject as T;
        }
    }
}