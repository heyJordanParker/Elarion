using Elarion.Managers;
using Elarion.UI;
using Elarion.UI.Animation;
using Elarion.UI.Animations;
using UnityEditor;

namespace Elarion.Editor {
    internal static class EditorMenus {
        [MenuItem("Tools/Elarion/Toggle Inspector Lock %l")] // Ctrl + L
        private static void ToggleInspectorLock() {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        [MenuItem("GameObject/UI/UI Manager")]
        private static void CreateUIManager() {
            Utils.Create<UIManager>();
        }

        // TODO use CreateAssetMenuAttribute
        [MenuItem("Assets/Create/UI Animation")]
        private static void SaveUIAnimation() {
            Utils.CreateScriptableObject<UIAnimation>();
        }
        
    }
}