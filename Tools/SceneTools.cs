using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Elarion.Tools {
    public static class SceneTools {
        
        public static List<T> FindSceneObjectsOfType<T>(bool includeInactive = true) {
            
            var results = new List<T>();
            
            for(var i = 0; i < SceneManager.sceneCount; ++i) {
                var scene = SceneManager.GetSceneAt(i);

                if(!scene.isLoaded) continue;
                
                var rootGameObjects = scene.GetRootGameObjects();
                
                foreach(var go in rootGameObjects) {
                    results.AddRange(go.GetComponentsInChildren<T>(includeInactive));
                }
            }

            return results;
        }
        
    }
}