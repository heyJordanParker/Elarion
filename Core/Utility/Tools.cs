using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Elarion.Utility {
    public static class Tools {
        
        public static List<T> FindSceneObjectsOfType<T>() {
            
            var results = new List<T>();
            
            for(var i = 0; i < SceneManager.sceneCount; ++i) {
                var scene = SceneManager.GetSceneAt(i);

                if(!scene.isLoaded) continue;
                
                var rootGameObjects = scene.GetRootGameObjects();
                
                foreach(var go in rootGameObjects) {
                    results.AddRange(go.GetComponentsInChildren<T>(includeInactive: true));
                }
            }

            return results;
        }
        
    }
}