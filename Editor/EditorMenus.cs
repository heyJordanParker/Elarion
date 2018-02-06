using Elarion.Managers;
using Elarion.UI;
using Elarion.UI.Animation;
using UnityEditor;

namespace Elarion.Editor {
    internal static class EditorMenus {
        [MenuItem("Tools/Elarion/Toggle Inspector Lock %l")] // Ctrl + L
        private static void ToggleInspectorLock() {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
        
        // TODO customize those so that they'll create intuitive to use objects
        // add animators, base hierarchy and so on (if it's missing)
        // add a TestClassUIRoot if there isn't one on the scene already
        [MenuItem("GameObject/UI/UI Element", false, -10)]
        private static void CreateUIElement() {
            Utils.Create<UIElement>();
            
            // creates a UIScenes object -> UIScene -> UIElement/
        }
        
        // TODO make sure this is fullscreen when created
        [MenuItem("GameObject/UI/UI Panel", false, -10)]
        private static void CreateUIPanel() {
            Utils.Create<UIPanel>();
            // create fullscreen element
            // Create UIRoot if missing
            
            // creates a UIScenes object -> UIScene -> UIPanel
        }
        
        // TODO use CreateAssetMenuAttribute
        [MenuItem("Assets/Create/UI Animation")]
        private static void SaveUIAnimation() {
            Utils.CreateScriptableObject<UIAnimation>();
        }
        
    }
}