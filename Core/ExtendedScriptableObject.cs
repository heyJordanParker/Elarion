using System;
using UnityEngine;

namespace Elarion {
    [Serializable]
    public class ExtendedScriptableObject : ScriptableObject {
#if UNITY_EDITOR

        protected static T Save<T>() where T : ExtendedScriptableObject {
            var path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
            var name = "New " + typeof(T).Name + ".asset";
            var separator = System.IO.Path.DirectorySeparatorChar;

            if(path == "") {
                path = "Assets" + separator + name;
            } else {
                if(System.IO.Directory.Exists(path)) {
                    path += separator;
                } else {
                    path = path.Substring(0, path.Length - System.IO.Path.GetFileName(path).Length);    
                }
                path += name;
            }
            
            var savedObject = CreateInstance(typeof(T));
            
            var savePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
            UnityEditor.AssetDatabase.CreateAsset(savedObject, savePath);
            UnityEditor.Selection.activeObject = savedObject;
            return savedObject as T;
        }

#endif
    }
}