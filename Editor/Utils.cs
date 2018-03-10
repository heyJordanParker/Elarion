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
        
        public static void ShowBuiltinHelpers(UIComponent component) {
            component.Renderer.hideFlags = HideFlags.None;
            component.GetComponent<GraphicRaycaster>().hideFlags = HideFlags.None;
            if(component.HasComponent<CanvasRenderer>()) {
                component.GetComponent<CanvasRenderer>().hideFlags = HideFlags.None;
            }
            
            if(component.HasComponent<CanvasGroup>()) {
                component.GetComponent<CanvasGroup>().hideFlags = HideFlags.None;
            }
            
            var scene = component as UIScene;

            if(scene) {
                scene.CanvasGroup.hideFlags = HideFlags.None;
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

            var scene = component as UIScene;

            if(scene) {
                scene.CanvasGroup.hideFlags = HideFlags.HideInInspector;
            }
        }

        public static Dictionary<Type, Component> GetComponentsDictionary(GameObject gameObject, IEnumerable<Type> componentTypes) {
            var helpers = new Dictionary<Type, Component>();
            foreach(var helperComponent in componentTypes) {
                helpers.Add(helperComponent, gameObject.GetComponent(helperComponent));
            }

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