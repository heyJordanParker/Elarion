using System.Collections.Generic;
using System.Linq;
using Elarion.Extensions;
using Elarion.UI;
using Elarion.Utility;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI {
    
    [InitializeOnLoad]
    public static class HierarchyValidator {

        private static List<UIScene> _uiScenes = SceneTools.FindSceneObjectsOfType<UIScene>();
        
        static HierarchyValidator() {
            EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate() {
            var initialScene = _uiScenes.SingleOrDefault(s => s.InitialScene);

            if(initialScene == null) {
                initialScene = _uiScenes.FirstOrDefault();
            }

            if(initialScene == null) {
                return;
            }
            
            if(!initialScene.gameObject.activeInHierarchy) {
                Debug.LogWarning("The initial scene is used for initialization and cannot be disabled. Enabling it and all its' parents.", initialScene.gameObject);

                var t = initialScene.transform;
                
                while(t != null && !t.gameObject.activeInHierarchy) {
                    t.gameObject.SetActive(true);

                    t = t.transform.parent;
                }
            }
        }

        private static void OnHierarchyChanged() {
            if(EditorApplication.isPlayingOrWillChangePlaymode) {
                return;
            }

            _uiScenes = SceneTools.FindSceneObjectsOfType<UIScene>();
            
            foreach(var scene in _uiScenes) {
                // check for nesting
                foreach(var parentScene in _uiScenes) {
                    if(scene == parentScene) {
                        continue;
                    }
                    
                    if(!parentScene.transform.IsParentOf(scene.transform)) continue;

                    scene.transform.SetParent(parentScene.transform.parent, false);
                    Debug.Log("Nesting scenes is not allowed. Removing nesting.", scene.gameObject);
                    break;
                }
                
                // maximize
                var rectTransform = scene.transform as RectTransform;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.one;
            }
        }
    }
}