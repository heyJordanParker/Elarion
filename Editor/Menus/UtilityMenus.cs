using Elarion.UI;
using Elarion.Utility;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Menus {
    internal static class UtilityMenus {
        [MenuItem("Tools/Toggle Inspector Lock %l", priority = 10)] // Ctrl + L
        private static void ToggleInspectorLock() {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
        
        [MenuItem("Tools/Toggle builtin UIComponent helpers", priority = 10)] // Ctrl + L
        private static void ToggleUIComponentHelpers() {
            var hideHelpers = !EditorPrefs.GetBool(Consts.HideUIComponentHelpersKey);
            
            EditorPrefs.SetBool(Consts.HideUIComponentHelpersKey, hideHelpers);
            
            var components = SceneTools.FindSceneObjectsOfType<UIComponent>();
            Debug.Log(components.Count + " Toggling to " +EditorPrefs.GetBool(Consts.HideUIComponentHelpersKey));


            foreach(var component in components) {
                if(hideHelpers) {
                    Utils.HideHelpers(component);
                } else {
                    Utils.ShowHelpers(component);
                }
            }
        }
    }
}