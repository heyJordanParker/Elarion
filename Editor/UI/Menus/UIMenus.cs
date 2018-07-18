using Elarion.UI;
using Elarion.UI.Helpers.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.Editor.UI.Menus {
    internal static class UIMenus {
        private const string UILayer = "UI";

        // TODO customize those so that they'll create intuitive to use objects
        // add animators, base hierarchy and so on (if it's missing)
        // add a TestClassUIRoot if there isn't one on the scene already

        [MenuItem("GameObject/UI/Animated Panel", false, -10)]
        private static void CreateUIPanel() {
            var panel = Create<UIPanel>("Panel");
            panel.gameObject.AddComponent<Image>().color = Color.cyan;
            panel.gameObject.AddComponent<UIAnimator>();
        }

        [MenuItem("GameObject/UI/Scene", false, -10)]
        private static void CreateUIScene() {
            var scene = Create<UIPanel>("Scene");
            scene.gameObject.AddComponent<Image>();
            scene.gameObject.AddComponent<UIAnimator>();
            scene.gameObject.AddComponent<UIComponentGroup>();
        }

        [MenuItem("GameObject/UI/UI Root", false, -10)]
        private static void CreateUIRoot(MenuCommand menuCommand) {
            var go = CreateNewUI();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            
            if (go.transform.parent as RectTransform)
            {
                RectTransform rect = go.transform as RectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
            }
            
            Selection.activeGameObject = go;
        }

        private static GameObject CreateNewUI() {
            var rootComponents = new[] {typeof(UIManager), typeof(Image), typeof(CanvasScaler)};

            var rootGO = new GameObject("UI Root", rootComponents) {
                layer = LayerMask.NameToLayer(UILayer)
            };

            rootGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Undo.RegisterCreatedObjectUndo(rootGO, "Creating " + rootGO.name);
            
            var scene = Create<UIPanel>("First Scene", rootGO);

            CreateEventSystem();
            
            return scene.gameObject;
        }

        private static void CreateEventSystem() {
            var esys = Object.FindObjectOfType<EventSystem>();

            if(esys != null) return;

            var eventSystem = new GameObject("Event System");
            GameObjectUtility.SetParentAndAlign(eventSystem, null);
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            Undo.RegisterCreatedObjectUndo(eventSystem, "Creating " + eventSystem.name);
        }

        // TODO move CreateUIElement to this and rename this to CreateUIElement
        public static T Create<T>(string name = null, GameObject parent = null) where T : MonoBehaviour {

            if(parent == null) {
                parent = Selection.activeGameObject;
            }

            var defaultName = ObjectNames.NicifyVariableName(typeof(T).Name);

            if(string.IsNullOrEmpty(name)) {
                name = defaultName;
            }

            var go = new GameObject(name);

            GameObjectUtility.SetParentAndAlign(go, parent);

            var behavior = go.AddComponent<T>();

            Selection.activeGameObject = go;

            Undo.RegisterCreatedObjectUndo(go, "Creating " + defaultName);

            return behavior;
        }
    }
}