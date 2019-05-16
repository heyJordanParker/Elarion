using Elarion.Tools;
using Elarion.UI;
using UnityEditor;

namespace Elarion.Editor.UI.Menus {
    internal static class UtilityMenus {
        [MenuItem("Tools/Toggle Inspector Lock %l", priority = 10)] // Ctrl + L
        private static void ToggleInspectorLock() {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
        
        [MenuItem("Tools/Toggle Builtin UIComponent Helpers", priority = 10)]
        private static void ToggleUIComponentHelpers() {
            var hideHelpers = !EditorPrefs.GetBool(Consts.HideUIComponentHelpersKey);
            
            EditorPrefs.SetBool(Consts.HideUIComponentHelpersKey, hideHelpers);
            
            var components = SceneTools.FindSceneObjectsOfType<UIComponent>();

            foreach(var component in components) {
                if(hideHelpers) {
                    Utils.HideBuiltinHelpers(component);
                } else {
                    Utils.ShowBuiltinHelpers(component);
                }
            }
        }
    }
}