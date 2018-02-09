using UnityEditor;

namespace Elarion.Editor.Menus {
    internal static class UtilityMenus {
        [MenuItem("Tools/Elarion/Toggle Inspector Lock %l")] // Ctrl + L
        private static void ToggleInspectorLock() {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
    }
}