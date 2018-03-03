using System.Collections.Generic;
using Elarion.Extensions;
using Elarion.UI;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {
    
    [InitializeOnLoad]
    public static class HierarchyValidator {
        static HierarchyValidator() {
            EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged() {
            if(EditorApplication.isPlayingOrWillChangePlaymode) {
                return;
            }
            
            foreach(var scene in UIScene.Scenes) {
                // check for nesting
                foreach(var parentScene in UIScene.Scenes) {
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