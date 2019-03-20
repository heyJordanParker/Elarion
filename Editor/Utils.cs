using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Elarion.Editor.Extensions;
using Elarion.Extensions;
using Elarion.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Elarion.Editor {
    internal static class Utils {
        private const string LastAssetSavePathKey = "LastAssetSavePath";

        private static string _lastAssetSavePath;

        public static string LastAssetSavePath {
            get {
                if(_lastAssetSavePath == null) {
                    _lastAssetSavePath = EditorPrefs.GetString(LastAssetSavePathKey, "/Assets/");
                }

                return _lastAssetSavePath;
            }
            set {
                _lastAssetSavePath = value;
                EditorPrefs.SetString(LastAssetSavePathKey, _lastAssetSavePath);
            }
        }

        public static void ShowBuiltinHelpers(UIComponent component) {
            component.Renderer.hideFlags = HideFlags.None;
            component.GetComponent<GraphicRaycaster>().hideFlags = HideFlags.None;
            if(component.HasComponent<CanvasRenderer>()) {
                component.GetComponent<CanvasRenderer>().hideFlags = HideFlags.None;
            }

            if(component.HasComponent<CanvasGroup>()) {
                component.GetComponent<CanvasGroup>().hideFlags = HideFlags.None;
            }
        }

        public static void HideBuiltinHelpers(UIComponent component) {
            component.Renderer.hideFlags = HideFlags.HideInInspector;
            component.GetComponent<GraphicRaycaster>().hideFlags = HideFlags.HideInInspector;
            if(component.HasComponent<CanvasRenderer>()) {
                component.GetComponent<CanvasRenderer>().hideFlags = HideFlags.HideInInspector;
            }

            if(component.HasComponent<CanvasGroup>()) {
                component.GetComponent<CanvasGroup>().hideFlags = HideFlags.HideInInspector;
            }
        }

        public static Dictionary<Type, Component> GetComponentsDictionary(GameObject gameObject,
            IEnumerable<Type> componentTypes) {
            var helpers = new Dictionary<Type, Component>();
            foreach(var helperComponent in componentTypes) {
                helpers.Add(helperComponent, gameObject.GetComponent(helperComponent));
            }

            helpers.Add(typeof(Mask), gameObject.GetComponent<Mask>());

            return helpers;
        }

        public static List<Type> GetTypesWithAttribute<TAttribute>() where TAttribute : Attribute {
            var typesWithHelpAttribute =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsDefined(typeof(TAttribute), false)
                select type;

            return typesWithHelpAttribute.ToList();
        }

        public static void DisplayAsyncProgressBar(string loadingText, float progress) {
            AsyncProgressBar.Display(loadingText, progress);
        }

        public static void ClearProgressBar() {
            AsyncProgressBar.Clear();
        }

        public static void SetIconForObject(Object obj, Texture2D icon) {
            var assetPath = AssetDatabase.GetAssetPath(obj);

            if(string.IsNullOrEmpty(assetPath)) {
                return;
            }
            
            var guiUtility = typeof(EditorGUIUtility);
            var methodInfo = guiUtility.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
            methodInfo.Invoke(null, new object[] {obj, icon});

            EditorUtility.SetDirty(obj);
        }

        public static Texture2D GetIconForObject(Object obj) {
            var guiUtility = typeof(EditorGUIUtility);
            var methodInfo = guiUtility.GetMethod("GetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
            return methodInfo.Invoke(null, new object[] {obj}) as Texture2D;
        }

        public static T CreateScriptableObject<T>(bool showSaveFilePopup = false, bool selectObject = true)
            where T : ScriptableObject {
            return CreateScriptableObject(typeof(T), showSaveFilePopup, selectObject) as T;
        }

        public static ScriptableObject CreateScriptableObject(Type type, bool showSaveFilePopup = false,
            bool selectObject = true) {
            string path = String.Empty;
            string typeName = ObjectNames.NicifyVariableName(type.Name);

            if(showSaveFilePopup) {
                path = EditorUtility.SaveFilePanelInProject($"Create new {typeName} asset.", $"New {typeName}.asset",
                    "asset",
                    $"Choose a location to save the new {typeName}.", LastAssetSavePath);

                LastAssetSavePath = path.RemoveExtension();

                if(string.IsNullOrEmpty(path)) {
                    return null;
                }
            } else {
                path = AssetDatabase.GetAssetPath(Selection.activeObject);
                var name = $"New {typeName}.asset";
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
            }

            var savedObject = ScriptableObject.CreateInstance(type);

            var savePath = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(savedObject, savePath);

            AssetDatabase.SaveAssets();

            if(selectObject) {
                Selection.activeObject = savedObject;
            } else {
                EditorGUIUtility.PingObject(savedObject);
            }

            Undo.RegisterCreatedObjectUndo(savedObject, $"Creating {typeName}.");

            return savedObject;
        }
    }
}